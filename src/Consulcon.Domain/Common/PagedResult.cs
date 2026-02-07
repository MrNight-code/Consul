namespace Consulcon.Domain.Common;

/// Resultado paginado genérico para consultas con paginación
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public PagedResult() { }

    public PagedResult(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// Mapea los items a otro tipo usando un selector
    public PagedResult<TResult> Map<TResult>(Func<T, TResult> selector)
    {
        return new PagedResult<TResult>
        {
            Items = Items.Select(selector),
            Page = Page,
            PageSize = PageSize,
            TotalCount = TotalCount
        };
    }
}
