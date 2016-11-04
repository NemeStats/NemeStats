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
using BusinessLogic.Caching;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.EventTracking;
using BusinessLogic.Export;
using BusinessLogic.Facades;
using BusinessLogic.Jobs.BoardGameGeekBatchUpdateJobService;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Email;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Points;
using BusinessLogic.Logic.Users;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RollbarSharp;
using StructureMap;
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
                    scan.WithDefaultConventions();
                });

            Scan(s =>
            {
                s.AssemblyContainingType<IBoardGameGeekApiClient>();
                s.AssemblyContainingType<IBusinessLogicEventBus>();
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            this.For<IRollbarClient>().Use(new RollbarClient()).Singleton();

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
            For<IGamingGroupSaver>().Use<GamingGroupSaver>();

            For<IGamingGroupInviteConsumer>().Use<GamingGroupInviteConsumer>();
            For<IPlayerSummaryBuilder>().Use<PlayerSummaryBuilder>();

            For<IPlayerRepository>().Use<EntityFrameworkPlayerRepository>();

            For<IGameDefinitionRetriever>().Use<GameDefinitionRetriever>();

            For<IPlayedGameRetriever>().Use<PlayedGameRetriever>();

            For<IUserStore<ApplicationUser>>()
                .Use<UserStore<ApplicationUser>>();


            this.For<IGamingGroupRetriever>().Use<GamingGroupRetriever>();

            For<IPendingGamingGroupInvitationRetriever>().Use<PendingGamingGroupInvitationRetriever>();

            For<IPlayedGameSaver>().Use<PlayedGameSaver>();

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

            For<IPlayerDeleter>().Use<PlayerDeleter>();

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

            this.For<IBoardGameGeekUserSaver>().Use<BoardGameGeekUserSaver>();
            this.For<IBoardGameGeekGamesImporter>().Use<BoardGameGeekGamesImporter>();
            this.For<IBoardGameGeekBatchUpdateJobService>().Use<BoardGameGeekBatchUpdateJobService>();

            For<IPointsCalculator>().Use<PointsCalculator>();
            For<IWeightTierCalculator>().Use<WeightTierCalculator>();
            For<IWeightBonusCalculator>().Use<WeightBonusCalculator>();
            For<IGameDurationBonusCalculator>().Use<GameDurationBonusCalculator>();
            For<IRecentPublicGamesRetriever>().Use<RecentPublicGamesRetriever>();
            For<ITopGamingGroupsRetriever>().Use<TopGamingGroupsRetriever>();
            For<ITopPlayersRetriever>().Use<TopPlayersRetriever>();
            For<ITrendingGamesRetriever>().Use<TrendingGamesRetriever>();
            For<IPlayerAchievementRetriever>().Use<PlayerAchievementRetriever>();
            For<IRecentPlayerAchievementsUnlockedRetreiver>().Use<RecentPlayerAchievementsUnlockedRetreiver>();


            For<ICacheService>().Use<CacheService>();

            For<IDateUtilities>().Use<DateUtilities>();
        }


        #endregion
    }
}