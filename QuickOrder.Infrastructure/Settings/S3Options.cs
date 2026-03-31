namespace QuickOrder.Infrastructure.Settings;

public class S3Options
{
    public const string SectionName = "S3";

    public string BucketName { get; init; } = string.Empty;
    public string Region { get; init; } = "us-east-1";
}
