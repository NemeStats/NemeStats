using System;
using System.Data.Entity;
using System.Threading;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.Users;
using RollbarSharp;
using StructureMap;
using StructureMap.Web;

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
                x.For<DbContext>().Transient().Use<NemeStatsDbContext>();
                x.For<IDataContext>().Transient().Use<NemeStatsDataContext>();
                x.For<ApplicationUserManager>().Transient().Use<ApplicationUserManager>();

                x.For<BusinessLogicEventsHandlerFactory>().Singleton().Use(new BusinessLogicEventsHandlerFactory(
                    new HandlerFactoryConfiguration()
                        .AddHandlerAssembly(typeof(IBusinessLogicEventHandler<>).Assembly)
                        .AddMessageAssembly(typeof(IBusinessLogicEvent).Assembly)));

                x.For<IRollbarClient>().Use(new RollbarClient()).Singleton();

                x.Scan(s =>
                {
                    s.AssemblyContainingType<IAchievement>();
                    s.RegisterConcreteTypesAgainstTheFirstInterface();
                });
            });
        }
    }
}