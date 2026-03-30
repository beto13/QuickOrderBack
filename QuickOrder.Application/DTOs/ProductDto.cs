namespace QuickOrder.Application.DTOs;

public record MenuProductDto(int Id, string Name, string? Description, decimal Price, int CategoryId, string CategoryName);
