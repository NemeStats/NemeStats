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

using BusinessLogic.Logic.BoardGameGeek;
using Microsoft.Owin.Security;

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
    using System.Configuration.Abstractions;
    using System.Data.Entity;
    using UI.Controllers.Helpers;
    using UI.Transformations;
    using UI.Transformations.PlayerTransformations;
    using UniversalAnalyticsHttpWrapper;
    using StructureMap.Web;
    using BusinessLogic.Logic.Nemeses;
    using System.Web;

    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });

            this.SetupSingletonMappings();

            this.SetupUniquePerRequestMappings();

            this.SetupTransientMappings();
        }

        private void SetupSingletonMappings()
        {
            this.For<IGameResultViewModelBuilder>().Singleton().Use<GameResultViewModelBuilder>();

            this.For<IPlayerDetailsViewModelBuilder>().Singleton().Use<PlayerDetailsViewModelBuilder>();

            this.For<IGamingGroupViewModelBuilder>().Singleton()
                                               .Use<GamingGroupViewModelBuilder>();

            this.For<IGamingGroupInvitationViewModelBuilder>().Singleton()
                                                         .Use<GamingGroupInvitationViewModelBuilder>();

            this.For<ITopPlayerViewModelBuilder>().Use<TopPlayerViewModelBuilder>();

            this.For<IPlayedGameDetailsViewModelBuilder>().Singleton().Use<PlayedGameDetailsViewModelBuilder>();

            this.For<IPlayerWithNemesisViewModelBuilder>().Singleton().Use<PlayerWithNemesisViewModelBuilder>();

            this.For<IMinionViewModelBuilder>().Singleton().Use<MinionViewModelBuilder>();

            this.For<INemesisChangeViewModelBuilder>().Singleton().Use<NemesisChangeViewModelBuilder>();

            this.For<IGameDefinitionDetailsViewModelBuilder>().Singleton().Use<GameDefinitionDetailsViewModelBuilder>();

            this.For<IGameDefinitionSummaryViewModelBuilder>().Singleton().Use<GameDefinitionSummaryViewModelBuilder>();
        }

        private void SetupTransientMappings()
        {
            this.For<IGamingGroupSaver>().Use<GamingGroupSaver>();

            this.For<IGamingGroupAccessGranter>().Use<EntityFrameworkGamingGroupAccessGranter>();

            this.For<IGamingGroupInviteConsumer>().Use<GamingGroupInviteConsumer>();
            this.For<IPlayerSummaryBuilder>().Use<PlayerSummaryBuilder>();

            this.For<IPlayerRepository>().Use<EntityFrameworkPlayerRepository>();

            this.For<IGameDefinitionRetriever>().Use<GameDefinitionRetriever>();

            this.For<IPlayedGameRetriever>().Use<PlayedGameRetriever>();

            this.For<Microsoft.AspNet.Identity.IUserStore<ApplicationUser>>()
                .Use<Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>>();

            this.For<IShowingXResultsMessageBuilder>().Use<ShowingXResultsMessageBuilder>();
            this.For<IGamingGroupRetriever>().Use<GamingGroupRetriever>();

            this.For<IPendingGamingGroupInvitationRetriever>().Use<PendingGamingGroupInvitationRetriever>();

            this.For<IPlayedGameCreator>().Use<PlayedGameCreator>();

            this.For<INemeStatsEventTracker>().Use<UniversalAnalyticsNemeStatsEventTracker>();

            this.For<IEventTracker>().Use<EventTracker>();
            this.For<INemesisHistoryRetriever>().Use<NemesisHistoryRetriever>();

            //TODO should never be injected by the IoC... need to confirm
            //For<IUniversalAnalyticsEvent>().Use<UniversalAnalyticsEvent>();

            this.For<IUniversalAnalyticsEventFactory>().Use<UniversalAnalyticsEventFactory>();

            this.For<IConfigurationManager>().Use(() => ConfigurationManager.Instance);

            this.For<IPlayerSaver>().Use<PlayerSaver>();

            this.For<IGameDefinitionSaver>().Use<GameDefinitionSaver>();

            this.For<IPlayerRetriever>().Use<PlayerRetriever>();

            this.For<INemesisRecalculator>().Use<NemesisRecalculator>();

            this.For<IPlayedGameDeleter>().Use<PlayedGameDeleter>();

            this.For<IUserRegisterer>().Use<UserRegisterer>();

            this.For<IFirstTimeAuthenticator>().Use<FirstTimeAuthenticator>();

            this.For<IBoardGameGeekSearcher>().Use<BoardGameGeekSearcher>();
        }

        private void SetupUniquePerRequestMappings()
        {
            this.For<DbContext>().HttpContextScoped().Use<NemeStatsDbContext>();
            this.For<IDataContext>().HttpContextScoped().Use<NemeStatsDataContext>();
            this.For<ApplicationUserManager>().HttpContextScoped().Use<ApplicationUserManager>();
            this.For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);
        }

        #endregion
    }
}