using NemeStats.IoC;
using StructureMap;

namespace BusinessLogic.Tests.IntegrationTests
{
    public abstract class IntegrationTestIoCBase
    {
        private static readonly IContainer RootContainer = new Container(c =>
        {
            c.AddRegistry<CommonRegistry>();
            c.AddRegistry<DatabaseRegistry>();
        });

        private readonly IContainer _container
            = RootContainer.GetNestedContainer();

        protected T GetInstance<T>()
        {
            return _container.GetInstance<T>();
        }

        protected void Inject<T>(T instance) where T : class
        {
            _container.Configure(cfg => cfg.For<T>().Use(instance));
        }
    }
}
