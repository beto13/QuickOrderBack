namespace QuickOrder.Application.DTOs;

// Tables
public record TableDto(int Id, string Number, bool IsActive);
public record CreateTableRequest(string Number);
public record UpdateTableRequest(string? Number, bool? IsActive);

// Menus
public record MenuDto(int Id, string Name, bool IsActive);
public record CreateMenuRequest(string Name);
public record UpdateMenuRequest(string? Name, bool? IsActive);

// Products
public record ProductDto(int Id, string Name, string? Description);
public record CreateProductRequest(string Name, string? Description);
public record UpdateProductRequest(string? Name, string? Description);

// Categories
public record CategoryDto(int Id, string Name, int DisplayOrder);
public record CreateCategoryRequest(string Name, int DisplayOrder);
public record UpdateCategoryRequest(string? Name, int? DisplayOrder);

// Modifier Groups
public record ModifierGroupDto(int Id, string Name, int MinSelections, int MaxSelections, bool IsRequired, int? ProductId, int? CategoryId);
public record CreateModifierGroupRequest(string Name, int MinSelections, int MaxSelections, bool IsRequired, int? ProductId, int? CategoryId);
public record UpdateModifierGroupRequest(string? Name, int? MinSelections, int? MaxSelections, bool? IsRequired);

// Modifiers
public record ModifierDto(int Id, int ModifierGroupId, string Name, string? Description);
public record CreateModifierRequest(int ModifierGroupId, string Name, string? Description);
public record UpdateModifierRequest(string? Name, string? Description);
