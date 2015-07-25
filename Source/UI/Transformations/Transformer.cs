using System.Linq;
using AutoMapper;

namespace UI.Transformations
{
    public class Transformer : ITransformer
    {
        public TDestination Transform<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TDestination>(source);
        }
    }
}