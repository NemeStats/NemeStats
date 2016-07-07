using StructureMap;
using UI.Mappers.Interfaces;

namespace UI.Mappers
{
    public class MapperFactory : IMapperFactory
    {
        private readonly IContainer _container;

        public MapperFactory(IContainer container)
        {
            _container = container;
        }

        public ICustomMapper<TSource, TResult> GetMapper<TSource, TResult>() where TSource : class
        {

            return _container.GetInstance<ICustomMapper<TSource, TResult>>();
        }
    }
}