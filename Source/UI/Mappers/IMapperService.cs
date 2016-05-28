using System.Collections.Generic;

namespace UI.Mappers
{
    public interface IMapperService<in TSource, out TResult> where TSource: class  
    {
        TResult Map(TSource source);
        IEnumerable<TResult> Map(IEnumerable<TSource> source);
    }
}