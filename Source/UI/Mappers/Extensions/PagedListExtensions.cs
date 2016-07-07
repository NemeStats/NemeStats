using PagedList;
using UI.Mappers.Interfaces;

namespace UI.Mappers.Extensions
{
    public static class PagedListExtensions
    {
        public static IPagedList<TResult> ToMappedPagedList<TSource, TResult>(this IPagedList<TSource> list, ICustomMapper<TSource, TResult> mapper) where TSource : class
        {
            var sourceList = mapper.Map(list);
            return new StaticPagedList<TResult>(sourceList, list.GetMetaData());
        }
    }
}