using FluentValidation;
using QuickOrder.Application.Features.Products.Commands;

namespace QuickOrder.Application.Features.Products.Validators;

public class UploadProductImageValidator : AbstractValidator<UploadProductImageCommand>
{
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public UploadProductImageValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("El ID del producto debe ser mayor a 0.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("La imagen no puede estar vacía.")
            .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage("La imagen no puede superar los 5 MB.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("El tipo de contenido es requerido.")
            .Must(ct => AllowedContentTypes.Contains(ct.ToLowerInvariant()))
            .WithMessage("Solo se permiten imágenes JPG, PNG o WebP.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("El nombre del archivo es requerido.");
    }
}
