using System;
using System.Threading;
using BusinessLogic.Events.Interfaces;
using StructureMap;

namespace BusinessLogic.Events.HandlerFactory
{
    public static class EventHandlerObjectFactory
    {
        private static readonly Lazy<Container> _containerBuilder =
            new Lazy<Container>(defaultContainer, LazyThreadSafetyMode.ExecutionAndPublication);

        public static IContainer Container => _containerBuilder.Value;

        private static Container defaultContainer()
        {
            return new Container(x =>
            {
                x.For<BusinenessLogicEventsHandlerFactory>().Singleton().Use(new BusinenessLogicEventsHandlerFactory(
                    new HandlerFactoryConfiguration()
                        .AddHandlerAssembly(typeof(IBusinessLogicEventHandler<>).Assembly)
                        .AddMessageAssembly(typeof(IBusinessLogicEvent).Assembly)));
            });
        }
    }
}