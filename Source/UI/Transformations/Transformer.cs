using BusinessLogic.Logic;

namespace UI.Transformations
{
    public class Transformer : ITransformer
    {
        public TDestination Transform<TDestination>(object source)
        {
            return AutomapperConfiguration.Mapper.Map<TDestination>(source);
        }
    }
}