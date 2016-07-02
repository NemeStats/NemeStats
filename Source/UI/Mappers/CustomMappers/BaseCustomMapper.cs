using System.Collections.Generic;
using AutoMapper;
using UI.Mappers.Interfaces;

namespace UI.Mappers.CustomMappers
{
    public abstract class BaseCustomMapper<TSource, TResult> : ICustomMapper<TSource, TResult> where TSource : class
    {
        public virtual TResult Map(TSource source)
        {
            return Mapper.Map<TResult>(source);
        }

        public virtual IEnumerable<TResult> Map(IEnumerable<TSource> source)
        {
            return Mapper.Map<IEnumerable<TResult>>(source);
        }
    }


}