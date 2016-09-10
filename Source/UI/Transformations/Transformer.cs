using AutoMapper;

namespace UI.Transformations
{
    public class Transformer : ITransformer
    {
        public TDestination Transform<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }
    }
}