using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using QuickOrder.Infrastructure.Persistence;
using QuickOrder.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IAmazonSQS>(_ =>
{
    var region = Amazon.RegionEndpoint.GetBySystemName(
        builder.Configuration["Sqs:Region"] ?? "us-east-1");
    return new AmazonSQSClient(region);
});

builder.Services.AddHostedService<OrderWorker>();

var host = builder.Build();
host.Run();
