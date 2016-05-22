using AutoMapper;

namespace UI.Mappers
{
    public abstract class BaseMapperService<TSource, TResult> : IMapperService<TSource, TResult> where TSource : class
    {
        public virtual TResult Map(TSource source)
        {
            return Mapper.Map<TResult>(source);
        }
    }
}