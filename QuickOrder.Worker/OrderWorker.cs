using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.SignalR.Client;
using QuickOrder.Application.Messages;

namespace QuickOrder.Worker;

public class OrderWorker(
    IAmazonSQS sqsClient,
    IConfiguration configuration,
    ILogger<OrderWorker> logger) : BackgroundService
{
    private readonly string _queueUrl = configuration["Sqs:QueueUrl"]
        ?? throw new InvalidOperationException("Sqs:QueueUrl no configurado.");
    private readonly string _hubUrl = configuration["SignalR:HubUrl"]
        ?? throw new InvalidOperationException("SignalR:HubUrl no configurado.");

    private HubConnection BuildHub() =>
        new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var hub = BuildHub();
        await ConnectHubAsync(hub, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20,
                    MessageAttributeNames = ["MessageType"]
                }, stoppingToken);

                foreach (var message in response.Messages)
                {
                    await ProcessMessageAsync(message, hub, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al leer mensajes de SQS");
                await Task.Delay(5000, stoppingToken);
            }
        }

        if (hub.State != HubConnectionState.Disconnected)
            await hub.StopAsync(stoppingToken);
    }

    private async Task ConnectHubAsync(HubConnection hub, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await hub.StartAsync(stoppingToken);
                logger.LogInformation("Conectado al hub SignalR en {Url}", _hubUrl);
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning("No se pudo conectar a SignalR ({Msg}). Reintentando en 5s...", ex.Message);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ProcessMessageAsync(Message message, HubConnection hub, CancellationToken cancellationToken)
    {
        message.MessageAttributes.TryGetValue("MessageType", out var typeAttr);
        var messageType = typeAttr?.StringValue;

        try
        {
            if (messageType == nameof(OrderCreatedEvent))
            {
                var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(message.Body);
                if (@event is not null)
                    await HandleOrderCreatedAsync(@event, hub, cancellationToken);
            }
            else
            {
                logger.LogWarning("Tipo de mensaje desconocido: {Type}", messageType);
            }

            await sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al procesar mensaje {MessageId}", message.MessageId);
        }
    }

    private async Task HandleOrderCreatedAsync(OrderCreatedEvent @event, HubConnection hub, CancellationToken cancellationToken)
    {
        logger.LogInformation("Nuevo pedido #{OrderId} recibido — mesa {Table}", @event.OrderId, @event.TableNumber);

        if (hub.State == HubConnectionState.Connected)
        {
            await hub.InvokeAsync("BroadcastNewOrder", @event.OrderId, cancellationToken);
        }
        else
        {
            logger.LogWarning("SignalR desconectado, no se pudo notificar pedido #{OrderId}", @event.OrderId);
        }
    }
}
