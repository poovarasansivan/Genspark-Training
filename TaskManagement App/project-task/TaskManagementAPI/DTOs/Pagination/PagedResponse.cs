namespace TaskManagementAPI.ApiResponses
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public string Message { get; set; } = string.Empty;
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();

        public static PagedResponse<T> Create(IEnumerable<T> data, int page, int pageSize, int totalRecords)
        {
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return new PagedResponse<T>
            {
                Data = data,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages
                }
            };
        }
    }
}
