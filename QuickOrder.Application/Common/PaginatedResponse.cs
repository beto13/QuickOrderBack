namespace QuickOrder.Application.Common;

public class PaginatedResponse<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PaginatedResponse<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
        => new() { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
}
