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

using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Export;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Email;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;

namespace UI.DependencyResolution
{
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
    using Controllers.Helpers;
    using Transformations;
    using Transformations.PlayerTransformations;
    using UniversalAnalyticsHttpWrapper;
    using StructureMap.Web;
    using BusinessLogic.Logic.Nemeses;
    using System.Web;
    using StructureMap;

    public class DefaultRegistry : Registry
    {
        #region Constructors and Destructors

        public DefaultRegistry()
        {
            this.Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });

            this.Scan(s =>
            {
                s.AssemblyContainingType<IBoardGameGeekApiClient>();
                s.RegisterConcreteTypesAgainstTheFirstInterface();
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

            this.For<IPlayerEditViewModelBuilder>().Singleton().Use<PlayerEditViewModelBuilder>();

            this.For<IDataProtectionProvider>().Singleton().Use<MachineKeyProtectionProvider>();

            this.For<ITransformer>().Singleton().Use<Transformer>();
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

            this.For<IUserStore<ApplicationUser>>()
                .Use<UserStore<ApplicationUser>>();

            this.For<IShowingXResultsMessageBuilder>().Use<ShowingXResultsMessageBuilder>();
            this.For<IGamingGroupRetriever>().Use<GamingGroupRetriever>();

            this.For<IPendingGamingGroupInvitationRetriever>().Use<PendingGamingGroupInvitationRetriever>();

            this.For<IPlayedGameCreator>().Use<PlayedGameCreator>();

            this.For<INemeStatsEventTracker>().Use<UniversalAnalyticsNemeStatsEventTracker>();

            this.For<IEventTracker>().Use<EventTracker>();
            this.For<INemesisHistoryRetriever>().Use<NemesisHistoryRetriever>();

            this.For<IUniversalAnalyticsEventFactory>().Use<UniversalAnalyticsEventFactory>();

            this.For<IConfigurationManager>().Use(() => ConfigurationManager.Instance);

            this.For<IPlayerSaver>().Use<PlayerSaver>();

            this.For<IGameDefinitionSaver>().Use<GameDefinitionSaver>();

            this.For<IPlayerRetriever>().Use<PlayerRetriever>();

            this.For<INemesisRecalculator>().Use<NemesisRecalculator>();

            this.For<IPlayedGameDeleter>().Use<PlayedGameDeleter>();

            this.For<IUserRegisterer>().Use<UserRegisterer>();

            this.For<IFirstTimeAuthenticator>().Use<FirstTimeAuthenticator>();

            this.For<IPlayerInviter>().Use<PlayerInviter>();

            this.For<IIdentityMessageService>().Use<EmailService>();

            this.For<IChampionRecalculator>().Use<ChampionRecalculator>();

            this.For<IChampionRepository>().Use<ChampionRepository>();

            this.For<IGamingGroupContextSwitcher>().Use<GamingGroupContextSwitcher>();

            this.For<IVotableFeatureRetriever>().Use<VotableFeatureRetriever>();

            this.For<IVotableFeatureVoter>().Use<VotableFeatureVoter>();

            this.For<IExcelGenerator>().Use<ExcelGenerator>();

            this.For<IAuthTokenGenerator>().Use<AuthTokenGenerator>();

            this.For<IAuthTokenValidator>().Use<AuthTokenValidator>();

            this.For(typeof(ISecuredEntityValidator<>)).Use(typeof(SecuredEntityValidator<>));

            this.For<IUserRetriever>().Use<UserRetriever>();

            this.For<IBoardGameGeekGameDefinitionCreator>().Use<BoardGameGeekGameDefinitionCreator>();

            this.For<IBoardGameGeekUserSaver>().Use<BoardGameGeekUserSaver>();
            this.For<IBoardGameGeekGamesImporter>().Use<BoardGameGeekGamesImporter>();
            this.For<IBoardGameGeekCleanUpService>().Use<BoardGameGeekCleanUpService>();

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