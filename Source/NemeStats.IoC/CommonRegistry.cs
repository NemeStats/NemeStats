#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System.Configuration.Abstractions;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Email;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RollbarSharp;
using StructureMap;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using UniversalAnalyticsHttpWrapper;

namespace NemeStats.IoC
{
    public class CommonRegistry : Registry
    {
        #region Constructors and Destructors

        public CommonRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.AssemblyContainingType<IBoardGameGeekApiClient>();
                    scan.AssemblyContainingType<IBusinessLogicEventBus>();
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                    scan.WithDefaultConventions();
                });

            

            For<IRollbarClient>().Use(new RollbarClient()).Singleton();

            SetupTransientMappings();

            SetupSpecialMappings();
        }

        private void SetupSpecialMappings()
        {
            var busHandlerConfiguration = new HandlerFactoryConfiguration()
                        .AddHandlerAssembly(typeof(IBusinessLogicEventHandler<>).Assembly)
                        .AddMessageAssembly(typeof(IBusinessLogicEvent).Assembly);
            For<HandlerFactoryConfiguration>().Use(busHandlerConfiguration).Singleton();
        }


        private void SetupTransientMappings()
        {
            For<IPlayerRepository>().Use<EntityFrameworkPlayerRepository>();
            
            For<IUserStore<ApplicationUser>>()
                .Use<UserStore<ApplicationUser>>();


            For<IUniversalAnalyticsEventFactory>().Use<UniversalAnalyticsEventFactory>();
            For<IEventTracker>().Use<EventTracker>();
            For<INemeStatsEventTracker>().Use<UniversalAnalyticsNemeStatsEventTracker>();

            For<IConfigurationManager>().Use(() => ConfigurationManager.Instance);
        
            For<IIdentityMessageService>().Use<EmailService>();

            For(typeof(ISecuredEntityValidator)).Use(typeof(SecuredEntityValidator));
        }


        #endregion
    }
}