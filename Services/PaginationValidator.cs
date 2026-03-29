namespace CarDealershipManager.Services
{
    public class PaginationValidator
    {
        private const int DefaultPageSize = 10;
        private static readonly int[] AllowedPageSizes = { 10, 20, 50 };
        public static int ValidatePageSize(int pageSize)
        {
            return AllowedPageSizes.Contains(pageSize) ? pageSize : DefaultPageSize;
        }
    }
}
