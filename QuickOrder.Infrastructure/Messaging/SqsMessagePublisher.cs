using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using QuickOrder.Application.Interfaces;
using QuickOrder.Infrastructure.Settings;

namespace QuickOrder.Infrastructure.Messaging;

public class SqsMessagePublisher(IAmazonSQS sqsClient, IOptions<SqsOptions> options) : IMessagePublisher
{
    private readonly string _queueUrl = string.IsNullOrWhiteSpace(options.Value.QueueUrl)
        ? throw new InvalidOperationException("Sqs:QueueUrl no configurado.")
        : options.Value.QueueUrl;

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        var body = JsonSerializer.Serialize(message);
        var messageType = typeof(T).Name;

        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = body,
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                ["MessageType"] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = messageType
                }
            }
        };

        await sqsClient.SendMessageAsync(request, cancellationToken);
    }
}
