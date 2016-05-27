namespace BusinessLogic.Paging
{
    public class PagedQuery
    {
        public PagedQuery()
        {
            Page = 1;
            PageSize = 10;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
