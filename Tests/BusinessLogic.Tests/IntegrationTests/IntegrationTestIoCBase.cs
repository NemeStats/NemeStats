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

        private readonly IContainer _nestedContainer
            = RootContainer.GetNestedContainer();

        /// <summary>
        /// Returns an instance from the nested container. This will get automatically disposed 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetInstance<T>()
        {
            return _nestedContainer.GetInstance<T>();
        }

        /// <summary>
        /// Returns an instance from the root container that will NOT automatically be disposed. You must dispose it yourself (or keep in a using statement ideally)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetInstanceFromRootContainer<T>()
        {
            return RootContainer.GetInstance<T>();
        }

        protected void Inject<T>(T instance) where T : class
        {
            _nestedContainer.Configure(cfg => cfg.For<T>().Use(instance));
        }

        protected void DisposeNestedContainer()
        {
            _nestedContainer.Dispose();
        }
    }
}
