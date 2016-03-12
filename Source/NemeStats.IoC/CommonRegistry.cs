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
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Export;
using BusinessLogic.Jobs.BoardGameGeekCleanUpService;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Email;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Users;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RollbarSharp;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Web;
using UniversalAnalyticsHttpWrapper;

namespace NemeStats.IoC
{
    public class CommonRegistry : Registry
    {
        #region Constructors and Destructors

        public CommonRegistry()
        {
            this.Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();                    
                });

            this.Scan(s =>
            {
                s.AssemblyContainingType<IBoardGameGeekApiClient>();
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            this.For<IRollbarClient>().Use(new RollbarClient()).Singleton();

            this.SetupUniquePerRequestMappings();

            this.SetupTransientMappings();
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
        }

        #endregion
    }
}