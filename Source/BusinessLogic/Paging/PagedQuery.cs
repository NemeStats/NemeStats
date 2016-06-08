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

        protected bool Equals(PagedQuery other)
        {
            return Page == other.Page && PageSize == other.PageSize;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PagedQuery) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Page*397) ^ PageSize;
            }
        }
    }

 
}
