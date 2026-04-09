using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using QuickOrder.Application.Interfaces;
using QuickOrder.Infrastructure.Settings;

namespace QuickOrder.Infrastructure.Storage;

public class S3StorageService(IAmazonS3 s3Client, IOptions<S3Options> options) : IStorageService
{
    private readonly S3Options _opts = options.Value;

    public async Task<string> UploadAsync(Stream content, string contentType, string key, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _opts.BucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType,
        };

        await s3Client.PutObjectAsync(request, cancellationToken);

        return $"https://{_opts.BucketName}.s3.{_opts.Region}.amazonaws.com/{key}";
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        await s3Client.DeleteObjectAsync(_opts.BucketName, key, cancellationToken);
    }
}
