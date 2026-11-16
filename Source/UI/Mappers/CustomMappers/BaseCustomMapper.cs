using System.Collections.Generic;
using UI.Mappers.Interfaces;
using UI.Transformations;

namespace UI.Mappers.CustomMappers
{
    public abstract class BaseCustomMapper<TSource, TResult> : ICustomMapper<TSource, TResult> where TSource : class
    {
        public virtual TResult Map(TSource source)
        {
            return AutomapperConfiguration.Mapper.Map<TResult>(source);
        }

        public virtual IEnumerable<TResult> Map(IEnumerable<TSource> source)
        {
            return AutomapperConfiguration.Mapper.Map<IEnumerable<TResult>>(source);
        }
    }


}