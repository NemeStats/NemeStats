using System;
using System.Data.Entity;
using System.Threading;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.Users;
using RollbarSharp;
using StructureMap;
using StructureMap.Web;
using UniversalAnalyticsHttpWrapper;

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
                x.For<DbContext>().HybridHttpOrThreadLocalScoped().Use<NemeStatsDbContext>();
                x.For<IDataContext>().HybridHttpOrThreadLocalScoped().Use<NemeStatsDataContext>();
                x.For<ApplicationUserManager>().HybridHttpOrThreadLocalScoped().Use<ApplicationUserManager>();

                x.For<BusinessLogicEventsHandlerFactory>().Singleton().Use(new BusinessLogicEventsHandlerFactory(
                    new HandlerFactoryConfiguration()
                        .AddHandlerAssembly(typeof(IBusinessLogicEventHandler<>).Assembly)
                        .AddMessageAssembly(typeof(IBusinessLogicEvent).Assembly)));

                x.For<IRollbarClient>().Use(new RollbarClient()).Singleton();

                x.For<IUniversalAnalyticsEventFactory>().Use<UniversalAnalyticsEventFactory>();
                x.For<IEventTracker>().Use<EventTracker>();
                x.For<INemeStatsEventTracker>().Use<UniversalAnalyticsNemeStatsEventTracker>();

                x.Scan(s =>
                {
                    s.AssemblyContainingType<IAchievement>();
                    s.RegisterConcreteTypesAgainstTheFirstInterface();
                });
            });
        }
    }
}