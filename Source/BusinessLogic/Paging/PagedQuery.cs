using PagedList;

namespace BusinessLogic.Paging
{
    public abstract class PagedQuery
    {
        protected PagedQuery()
        {
            Page = 1;
            PageSize = 10;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }

 
}
