namespace QuickOrder.Infrastructure.Settings;

public class SqsOptions
{
    public const string SectionName = "Sqs";

    public string Region { get; init; } = "us-east-1";
    public string QueueUrl { get; init; } = string.Empty;
}
