namespace QuickOrder.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(Stream content, string contentType, string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
