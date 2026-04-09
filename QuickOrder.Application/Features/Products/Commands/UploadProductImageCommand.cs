using MediatR;
using QuickOrder.Application.Common;
using QuickOrder.Application.Interfaces;

namespace QuickOrder.Application.Features.Products.Commands;

public record UploadProductImageCommand(
    int ProductId,
    Stream ImageStream,
    string ContentType,
    string FileName,
    long FileSize) : IRequest<Result<string>>;

public class UploadProductImageCommandHandler(
    IProductRepository productRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork) : IRequestHandler<UploadProductImageCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result<string>.Fail(Error.NotFound($"Producto {request.ProductId} no encontrado."));

        if (product.ImageUrl is not null)
        {
            var oldKey = ExtractKeyFromUrl(product.ImageUrl);
            if (oldKey is not null)
                await storageService.DeleteAsync(oldKey, cancellationToken);
        }

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var key = $"products/{request.ProductId}/{Guid.NewGuid()}{extension}";

        var url = await storageService.UploadAsync(request.ImageStream, request.ContentType, key, cancellationToken);

        product.ImageUrl = url;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Ok(url);
    }

    private static string? ExtractKeyFromUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
        return uri.AbsolutePath.TrimStart('/');
    }
}
