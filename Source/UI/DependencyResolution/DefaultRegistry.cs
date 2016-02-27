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
using System.Data.Entity;
using System.Web;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Export;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Email;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Points;
using BusinessLogic.Logic.Users;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models.User;
using BusinessLogic.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Web;
using UI.Controllers.Helpers;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;
using UniversalAnalyticsHttpWrapper;

namespace UI.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        #region Constructors and Destructors

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });

            Scan(s =>
            {
                s.AssemblyContainingType<IBoardGameGeekApiClient>();
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            SetupSingletonMappings();

            SetupUniquePerRequestMappings();

            SetupTransientMappings();
        }

        private void SetupSingletonMappings()
        {
            For<IGameResultViewModelBuilder>().Singleton().Use<GameResultViewModelBuilder>();

            For<IPlayerDetailsViewModelBuilder>().Singleton().Use<PlayerDetailsViewModelBuilder>();

            For<IGamingGroupViewModelBuilder>().Singleton()
                                               .Use<GamingGroupViewModelBuilder>();

            For<IGamingGroupInvitationViewModelBuilder>().Singleton()
                                                         .Use<GamingGroupInvitationViewModelBuilder>();

            For<ITopPlayerViewModelBuilder>().Use<TopPlayerViewModelBuilder>();

            For<IPlayedGameDetailsViewModelBuilder>().Singleton().Use<PlayedGameDetailsViewModelBuilder>();

            For<IPlayerWithNemesisViewModelBuilder>().Singleton().Use<PlayerWithNemesisViewModelBuilder>();

            For<IMinionViewModelBuilder>().Singleton().Use<MinionViewModelBuilder>();

            For<INemesisChangeViewModelBuilder>().Singleton().Use<NemesisChangeViewModelBuilder>();

            For<IGameDefinitionDetailsViewModelBuilder>().Singleton().Use<GameDefinitionDetailsViewModelBuilder>();

            For<IGameDefinitionSummaryViewModelBuilder>().Singleton().Use<GameDefinitionSummaryViewModelBuilder>();

            For<IPlayerEditViewModelBuilder>().Singleton().Use<PlayerEditViewModelBuilder>();

            For<IDataProtectionProvider>().Singleton().Use<MachineKeyProtectionProvider>();

            For<ITransformer>().Singleton().Use<Transformer>();

            For<IAverageGameDurationTierCalculator>().Singleton().Use<IAverageGameDurationTierCalculator>();

            For<IWeightTierCalculator>().Singleton().Use<WeightTierCalculator>();
        }

        private void SetupTransientMappings()
        {
            For<IGamingGroupSaver>().Use<GamingGroupSaver>();

            For<IGamingGroupAccessGranter>().Use<EntityFrameworkGamingGroupAccessGranter>();

            For<IGamingGroupInviteConsumer>().Use<GamingGroupInviteConsumer>();
            For<IPlayerSummaryBuilder>().Use<PlayerSummaryBuilder>();

            For<IPlayerRepository>().Use<EntityFrameworkPlayerRepository>();

            For<IGameDefinitionRetriever>().Use<GameDefinitionRetriever>();

            For<IPlayedGameRetriever>().Use<PlayedGameRetriever>();

            For<IUserStore<ApplicationUser>>()
                .Use<UserStore<ApplicationUser>>();

            For<IShowingXResultsMessageBuilder>().Use<ShowingXResultsMessageBuilder>();
            For<IGamingGroupRetriever>().Use<GamingGroupRetriever>();

            For<IPendingGamingGroupInvitationRetriever>().Use<PendingGamingGroupInvitationRetriever>();

            For<IPlayedGameCreator>().Use<PlayedGameCreator>();

            For<INemeStatsEventTracker>().Use<UniversalAnalyticsNemeStatsEventTracker>();

            For<IEventTracker>().Use<EventTracker>();
            For<INemesisHistoryRetriever>().Use<NemesisHistoryRetriever>();

            For<IUniversalAnalyticsEventFactory>().Use<UniversalAnalyticsEventFactory>();

            For<IConfigurationManager>().Use(() => ConfigurationManager.Instance);

            For<IPlayerSaver>().Use<PlayerSaver>();

            For<IGameDefinitionSaver>().Use<GameDefinitionSaver>();

            For<IPlayerRetriever>().Use<PlayerRetriever>();

            For<INemesisRecalculator>().Use<NemesisRecalculator>();

            For<IPlayedGameDeleter>().Use<PlayedGameDeleter>();

            For<IUserRegisterer>().Use<UserRegisterer>();

            For<IFirstTimeAuthenticator>().Use<FirstTimeAuthenticator>();

            For<IPlayerInviter>().Use<PlayerInviter>();

            For<IIdentityMessageService>().Use<EmailService>();

            For<IChampionRecalculator>().Use<ChampionRecalculator>();

            For<IChampionRepository>().Use<ChampionRepository>();

            For<IGamingGroupContextSwitcher>().Use<GamingGroupContextSwitcher>();

            For<IVotableFeatureRetriever>().Use<VotableFeatureRetriever>();

            For<IVotableFeatureVoter>().Use<VotableFeatureVoter>();

            For<IExcelGenerator>().Use<ExcelGenerator>();

            For<IAuthTokenGenerator>().Use<AuthTokenGenerator>();

            For<IAuthTokenValidator>().Use<AuthTokenValidator>();

            For(typeof(ISecuredEntityValidator<>)).Use(typeof(SecuredEntityValidator<>));

            For<IUserRetriever>().Use<UserRetriever>();

            For<IBoardGameGeekGameDefinitionCreator>().Use<BoardGameGeekGameDefinitionCreator>();

            For<IBoardGameGeekUserSaver>().Use<BoardGameGeekUserSaver>();
            For<IBoardGameGeekGamesImporter>().Use<BoardGameGeekGamesImporter>();

        }

        private void SetupUniquePerRequestMappings()
        {
            For<DbContext>().HttpContextScoped().Use<NemeStatsDbContext>();
            For<IDataContext>().HttpContextScoped().Use<NemeStatsDataContext>();
            For<ApplicationUserManager>().HttpContextScoped().Use<ApplicationUserManager>();
            For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);
        }

        #endregion
    }
}