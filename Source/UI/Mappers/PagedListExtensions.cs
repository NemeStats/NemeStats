using PagedList;

namespace UI.Mappers
{
    public static class PagedListExtensions
    {
        public static IPagedList<TResult> ToMappedPagedList<TSource, TResult>(this IPagedList<TSource> list, IMapperService<TSource, TResult> mapper) where TSource : class
        {
            var sourceList = mapper.Map(list);
            return new StaticPagedList<TResult>(sourceList, list.GetMetaData());
        }
    }
}