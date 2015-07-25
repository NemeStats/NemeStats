using System.Linq;

namespace UI.Transformations
{
    public interface ITransformer
    {
        TDestination Transform<TSource, TDestination>(TSource source);
    }
}
