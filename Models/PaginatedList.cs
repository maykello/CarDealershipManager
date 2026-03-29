namespace CarDealershipManager.Models
{
    public class PaginatedList<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        public PaginatedList(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentException("Page index must be greater than 0.", nameof(pageIndex));

            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));

            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            Items = items.ToList().AsReadOnly();
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
