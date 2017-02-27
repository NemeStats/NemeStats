using System.Linq;
using BusinessLogic.Logic;
using Microsoft.Ajax.Utilities;
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

        public static IPagedList<TResult> ToTransformedPagedList<TSource, TResult>(this IPagedList<TSource> list, ITransformer transformer) where TSource : class
        {
            var sourceList = list.Select(transformer.Transform<TResult>);
            return new StaticPagedList<TResult>(sourceList, list.GetMetaData());
        }
    }
}