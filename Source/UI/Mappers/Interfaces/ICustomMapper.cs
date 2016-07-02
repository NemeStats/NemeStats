using System.Collections.Generic;
using System.Diagnostics;

namespace UI.Mappers.Interfaces
{
    public interface ICustomMapper<in TSource, out TResult> where TSource : class
    {
        TResult Map(TSource source);
        IEnumerable<TResult> Map(IEnumerable<TSource> source);
    }
}