// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UI.DependencyResolution {
    using BusinessLogic.DataAccess;
    using BusinessLogic.DataAccess.GamingGroups;
    using BusinessLogic.DataAccess.Repositories;
    using BusinessLogic.EventTracking;
    using BusinessLogic.Logic.GameDefinitions;
    using BusinessLogic.Logic.GamingGroups;
    using BusinessLogic.Logic.PlayedGames;
    using BusinessLogic.Logic.Players;
    using BusinessLogic.Logic.Users;
    using BusinessLogic.Models.User;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using StructureMap.Pipeline;
    using System.Configuration.Abstractions;
    using System.Data.Entity;
    using UI.Controllers.Helpers;
    using UI.Transformations;
    using UI.Transformations.Player;
    using UniversalAnalyticsHttpWrapper;
	
    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
            //For<IExample>().Use<Example>();
            //unique per request scope

            For<DbContext>().LifecycleIs(new UniquePerRequestLifecycle()).Use<NemeStatsDbContext>();
            For<IDataContext>().LifecycleIs(new UniquePerRequestLifecycle()).Use<NemeStatsDataContext>();

            //transient scope
            For<IGamingGroupSaver>().Use<GamingGroupSaver>();

            For<IGamingGroupAccessGranter>().Use<EntityFrameworkGamingGroupAccessGranter>();

            For<IGamingGroupInviteConsumer>().Use<GamingGroupInviteConsumer>();
            For<IPlayerSummaryBuilder>().Use<PlayerSummaryBuilder>();

            For<IPlayerRepository>().Use<EntityFrameworkPlayerRepository>();

            For<IGameDefinitionRetriever>().Use<GameDefinitionRetriever>();

            For<IPlayedGameRetriever>().Use<PlayedGameRetriever>();

            For<Microsoft.AspNet.Identity.IUserStore<ApplicationUser>>()
                .Use<Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>>();

            For<IGameDefinitionViewModelBuilder>()
                .Use<GameDefinitionViewModelBuilder>();

            For<IShowingXResultsMessageBuilder>().Use<ShowingXResultsMessageBuilder>();
            For<IGamingGroupRetriever>().Use<GamingGroupRetriever>();

            For<IPendingGamingGroupInvitationRetriever>().Use<PendingGamingGroupInvitationRetriever>();

            For<IPlayedGameCreator>().Use<PlayedGameCreator>();

            For<NemeStatsEventTracker>().Use<UniversalAnalyticsNemeStatsEventTracker>();

            For<IEventTracker>().Use<EventTracker>();

            For<IUniversalAnalyticsEvent>().Use<UniversalAnalyticsEvent>();

            For<IUniversalAnalyticsEventFactory>().Use<UniversalAnalyticsEventFactory>();

            For<IConfigurationManager>().Use<ConfigurationManager>();

            For<IPlayerSaver>().Use<PlayerSaver>();

            For<IGameDefinitionSaver>().Use<GameDefinitionSaver>();

            For<IPlayerRetriever>().Use<PlayerRetriever>();



            //singleton scope

            For<IGameResultViewModelBuilder>().Singleton().Use<GameResultViewModelBuilder>();

            For<IPlayerDetailsViewModelBuilder>().Singleton().Use<PlayerDetailsViewModelBuilder>();

            For<IGamingGroupViewModelBuilder>().Singleton()

                .Use<GamingGroupViewModelBuilder>();

            For<IGamingGroupInvitationViewModelBuilder>().Singleton()

                .Use<GamingGroupInvitationViewModelBuilder>();

            For<ITopPlayerViewModelBuilder>().Use<TopPlayerViewModelBuilder>();

            For<IPlayedGameDetailsViewModelBuilder>().Singleton().Use<PlayedGameDetailsViewModelBuilder>();

        }

        #endregion
    }
}