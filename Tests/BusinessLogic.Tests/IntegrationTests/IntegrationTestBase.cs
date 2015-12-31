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
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture, Category("Integration")]
    public class IntegrationTestBase
    {
        protected SecuredEntityValidatorFactory securedEntityValidatorFactory = new SecuredEntityValidatorFactory();
        protected IEventTracker eventTrackerStub;
        protected IUniversalAnalyticsEventFactory eventFactory = new UniversalAnalyticsEventFactory();
        protected INemeStatsEventTracker playedGameTracker;

        protected List<PlayedGame> testPlayedGames = new List<PlayedGame>();
        protected GameDefinition testGameDefinition;
        protected GameDefinition testGameDefinition2;
        protected GameDefinition testGameDefinitionWithOtherGamingGroupId;
        protected GameDefinition anotherTestGameDefinitionWithOtherGamingGroupId;
        protected GameDefinition gameDefinitionWithNoChampion;
        protected ApplicationUser testUserWithDefaultGamingGroup;
        protected ApplicationUser testUserWithOtherGamingGroup;
        protected ApplicationUser testUserWithThirdGamingGroup;
        protected ApplicationUser testUserWithDefaultGamingGroupAndNoInvites;
        protected Player testPlayer1;
        protected string testPlayer1Name = "testPlayer1";
        protected Player testPlayer2;
        protected string testPlayer2Name = "testPlayer2";
        protected Player testPlayer3;
        protected string testPlayer3Name = "testPlayer3";
        protected Player testPlayer4;
        protected string testPlayer4Name = "testPlayer4";
        //player 1's nemesis
        protected Player testPlayer5;
        protected string testPlayer5Name = "testPlayer5";
        protected Player testPlayer6;
        protected string testPlayer6Name = "testPlayer6";
        protected Player testPlayer7WithOtherGamingGroupId;
        protected string testPlayer7Name = "testPlayer7";
        protected Player testPlayer8WithOtherGamingGroupId;
        protected string testPlayer8Name = "testPlayer8";
        protected Player testPlayer9UndefeatedWith5Games;
        protected string testPlayer9UndefeatedWith5GamesName = "testPlayer9";
        protected Player testPlayerWithNoPlayedGames;
        protected string testPlayerWithNoPlayedGamesName = "test player with no played games";
        protected string testGameName = "this is test game definition name";
        protected string testGameName2 = "aaa - game definition that should sort first";
        protected string testGameNameForGameWithOtherGamingGroupId = "this is test game definition name for game with other GamingGroupId";
        protected string gameDefinitionWithNoChampionName = "this is test game definition name for game with no champion";
        protected string testGameNameForAnotherGameWithOtherGamingGroupId = "test definition for game in other gaming group";
        protected string testGameDescription = "this is a test game description 123abc";
        protected string testApplicationUserNameForUserWithDefaultGamingGroup = "username with default gaming group";
        protected string testApplicationUserNameForUserWithOtherGamingGroup = "username with other gaming group";
        protected string testApplicationUserNameForUserWithDefaultGamingGroupAndNoInvites = "username with default gaming group and no invites";
        protected string testApplicationUserNameForUserWithThirdGamingGroup = "username with third gaming group";
        protected string testGamingGroup1Name = "this is test gaming group 1";
        protected string testGamingGroup2Name = "this is test gaming group 2";
        protected string testGamingGroup3Name = "this is test gaming group 3";
        protected GamingGroup testGamingGroup;
        protected GamingGroup testOtherGamingGroup;
        protected GamingGroup testThirdGamingGroup;
        protected string testInviteeEmail1 = "email1@email.com";
        protected GamingGroupInvitation testUnredeemedGamingGroupInvitation;
        protected string testInviteeEmail2 = "email2@email.com";
        protected GamingGroupInvitation testAlreadyRedeemedGamingGroupInvitation;
        protected List<int> nemesisIdsToDelete;
        protected List<int> championIdsToDelete;

        [OneTimeSetUp]
        public virtual void FixtureSetUp()
        {
            //create a stub for this only since we don't want the slowdown of all of the universal analytics event tracking
            eventTrackerStub = MockRepository.GenerateStub<IEventTracker>();
            eventTrackerStub.Expect(stub => stub.TrackEvent(Arg<IUniversalAnalyticsEvent>.Is.Anything))
                .Repeat.Any();
             
            playedGameTracker = new UniversalAnalyticsNemeStatsEventTracker(eventTrackerStub, eventFactory);

            using (NemeStatsDbContext nemeStatsDbContext = new NemeStatsDbContext())
            {
                CleanUpTestData();

                testUserWithDefaultGamingGroup = SaveApplicationUser(
                    nemeStatsDbContext,
                    testApplicationUserNameForUserWithDefaultGamingGroup,
                    "a@mailinator.com");
                testUserWithDefaultGamingGroupAndNoInvites = SaveApplicationUser(
                    nemeStatsDbContext,
                    testApplicationUserNameForUserWithDefaultGamingGroupAndNoInvites,
                    "a2@mailinator.com");
                testUserWithOtherGamingGroup = SaveApplicationUser(
                    nemeStatsDbContext,
                    testApplicationUserNameForUserWithOtherGamingGroup,
                    "b@mailinator.com");
                testUserWithThirdGamingGroup = SaveApplicationUser(
                    nemeStatsDbContext,
                    testApplicationUserNameForUserWithThirdGamingGroup, 
                    "c@mailinator.com");

                using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
                {
                    testGamingGroup = SaveGamingGroup(dataContext, testGamingGroup1Name, testUserWithDefaultGamingGroup);
                    testUserWithDefaultGamingGroup = UpdateDatefaultGamingGroupOnUser(testUserWithDefaultGamingGroup, testGamingGroup, dataContext);
                    testOtherGamingGroup = SaveGamingGroup(dataContext, testGamingGroup2Name, testUserWithOtherGamingGroup);
                    testUserWithOtherGamingGroup = UpdateDatefaultGamingGroupOnUser(testUserWithOtherGamingGroup, testOtherGamingGroup, dataContext);
                    testThirdGamingGroup = SaveGamingGroup(dataContext, testGamingGroup3Name, testUserWithThirdGamingGroup);
                    testUserWithThirdGamingGroup = UpdateDatefaultGamingGroupOnUser(testUserWithThirdGamingGroup,
                        testThirdGamingGroup, dataContext);

                    testGameDefinition = SaveGameDefinition(nemeStatsDbContext, testGamingGroup.Id, testGameName);
                    testGameDefinition2 = SaveGameDefinition(nemeStatsDbContext, testGamingGroup.Id, testGameName2);
                    testGameDefinitionWithOtherGamingGroupId = SaveGameDefinition(nemeStatsDbContext, testOtherGamingGroup.Id, testGameNameForGameWithOtherGamingGroupId);
                    gameDefinitionWithNoChampion = SaveGameDefinition(nemeStatsDbContext, testThirdGamingGroup.Id,
                        gameDefinitionWithNoChampionName);
                    anotherTestGameDefinitionWithOtherGamingGroupId = SaveGameDefinition(nemeStatsDbContext,
                        testOtherGamingGroup.Id, testGameNameForAnotherGameWithOtherGamingGroupId);
                    SavePlayers(nemeStatsDbContext, testGamingGroup.Id, testOtherGamingGroup.Id);

                    SaveGamingGroupInvitations(nemeStatsDbContext, dataContext);
                }

                using(NemeStatsDataContext dataContext = new NemeStatsDataContext())
                {
                    CreatePlayedGames(dataContext);
                }
            }
        }

        private void SaveGamingGroupInvitations(NemeStatsDbContext nemeStatsDbContext, IDataContext dataContext)
        {
            EntityFrameworkGamingGroupAccessGranter accessGranter = new EntityFrameworkGamingGroupAccessGranter(dataContext);
            testUnredeemedGamingGroupInvitation = accessGranter.CreateInvitation(testUserWithDefaultGamingGroup.Email, testUserWithDefaultGamingGroup);
            dataContext.CommitAllChanges();

            testAlreadyRedeemedGamingGroupInvitation = accessGranter.CreateInvitation(testUserWithOtherGamingGroup.Email, testUserWithDefaultGamingGroup);
            //TODO simulating registration. Will need a separate method for this soon so this logic can be replaced
            testAlreadyRedeemedGamingGroupInvitation.DateRegistered = DateTime.UtcNow.AddDays(1);
            testAlreadyRedeemedGamingGroupInvitation.RegisteredUserId = testUserWithOtherGamingGroup.Id;
            nemeStatsDbContext.GamingGroupInvitations.Add(testAlreadyRedeemedGamingGroupInvitation);
            nemeStatsDbContext.SaveChanges();
        }

        private ApplicationUser UpdateDatefaultGamingGroupOnUser(ApplicationUser user, GamingGroup gamingGroup, NemeStatsDataContext dataContext)
        {
            user.CurrentGamingGroupId = gamingGroup.Id;
            dataContext.CommitAllChanges();

            return user;
        }

        private void CreatePlayedGames(NemeStatsDataContext dataContext)
        {
            IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
            INemesisRecalculator nemesisRecalculator = new NemesisRecalculator(dataContext, playerRepository);
            IChampionRepository championRepository = new ChampionRepository(dataContext);
            IChampionRecalculator championRecalculator = new ChampionRecalculator(dataContext, championRepository);
            ISecuredEntityValidator<Player> securedEntityValidatorForPlayers = new SecuredEntityValidator<Player>();
            ISecuredEntityValidator<GameDefinition> securedEntityValidatorForGameDefinition = new SecuredEntityValidator<GameDefinition>();
            IPlayedGameCreator playedGameCreator = new PlayedGameCreator(
                dataContext, 
                playedGameTracker, 
                nemesisRecalculator, 
                championRecalculator, 
                securedEntityValidatorForPlayers,
                securedEntityValidatorForGameDefinition);
            
            List<Player> players = new List<Player>() { testPlayer1, testPlayer2 };
            List<int> playerRanks = new List<int>() { 1, 1 };
            PlayedGame playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int>() { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer1, testPlayer3, testPlayer2 };
            playerRanks = new List<int>() { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer3, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            //make player4 beat player 1 three times
            players = new List<Player>() { testPlayer4, testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int>() { 1, 2, 3, 4 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer4, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer4, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            //--make the inactive player5 beat player1 3 times
            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            //make player 2 be the only one who beat player 5
            players = new List<Player>() { testPlayer2, testPlayer5 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            //--create games that have a different GamingGroupId and testPlayer7 being the champion
            players = new List<Player>() { testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer7WithOtherGamingGroupId, testPlayer8WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer7WithOtherGamingGroupId, testPlayer8WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer7WithOtherGamingGroupId, testPlayer8WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer7WithOtherGamingGroupId, testPlayer8WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer8WithOtherGamingGroupId, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, playedGameCreator);
            testPlayedGames.Add(playedGame);
        }

        private void SavePlayers(NemeStatsDbContext nemeStatsDbContext, int primaryGamingGroupId, int otherGamingGroupId)
        {
            testPlayer1 = new Player() { Name = testPlayer1Name, Active = true, GamingGroupId = primaryGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayer1);
            testPlayer2 = new Player() { Name = testPlayer2Name, Active = true, GamingGroupId = primaryGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayer2);
            testPlayer3 = new Player() { Name = testPlayer3Name, Active = true, GamingGroupId = primaryGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayer3);
            testPlayer4 = new Player() { Name = testPlayer4Name, Active = true, GamingGroupId = primaryGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayer4);
            testPlayer5 = new Player() { Name = testPlayer5Name, Active = false, GamingGroupId = primaryGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayer5);
            testPlayer6 = new Player() { Name = testPlayer6Name, Active = true, GamingGroupId = primaryGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayer6);

            testPlayer7WithOtherGamingGroupId = new Player() { Name = testPlayer7Name, Active = true, GamingGroupId = otherGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayer7WithOtherGamingGroupId);
            testPlayer8WithOtherGamingGroupId = new Player() { Name = testPlayer8Name, Active = true, GamingGroupId = otherGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayer8WithOtherGamingGroupId);
            testPlayer9UndefeatedWith5Games = new Player { Name = testPlayer9UndefeatedWith5GamesName, Active = false, GamingGroupId =  otherGamingGroupId};
            nemeStatsDbContext.Players.Add(testPlayer9UndefeatedWith5Games);

            testPlayerWithNoPlayedGames = new Player { Name = testPlayerWithNoPlayedGamesName, Active = true, GamingGroupId = primaryGamingGroupId };
            nemeStatsDbContext.Players.Add(testPlayerWithNoPlayedGames);

            nemeStatsDbContext.SaveChanges();
        }

        private GameDefinition SaveGameDefinition(NemeStatsDbContext nemeStatsDbContext, int gamingGroupId, string gameDefinitionName)
        {
            GameDefinition gameDefinition = new GameDefinition() { Name = gameDefinitionName, Description = testGameDescription, GamingGroupId = gamingGroupId };
            nemeStatsDbContext.GameDefinitions.Add(gameDefinition);
            nemeStatsDbContext.SaveChanges();

            return gameDefinition;
        }

        private ApplicationUser SaveApplicationUser(NemeStatsDbContext nemeStatsDbContext, string userName, string email)
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = email,
                UserName = userName,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                AnonymousClientId = new Guid().ToString()
            };
            nemeStatsDbContext.Users.Add(applicationUser);
            nemeStatsDbContext.SaveChanges();

            return applicationUser;
        }

        private GamingGroup SaveGamingGroup(NemeStatsDataContext dataContext, string gamingGroupName, ApplicationUser owningUser)
        {
            GamingGroup gamingGroup = new GamingGroup() { Name = gamingGroupName, OwningUserId = owningUser.Id };
            dataContext.Save(gamingGroup, owningUser);
            dataContext.CommitAllChanges();

            return gamingGroup;
        }

        private PlayedGame CreateTestPlayedGame(
            int gameDefinitionId,
            List<Player> players,
            List<int> correspondingPlayerRanks,
            ApplicationUser currentUser,
            IPlayedGameCreator playedGameCreator)
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();

            for (int i = 0; i < players.Count(); i++)
            {
                playerRanks.Add(new PlayerRank()
                    {
                        PlayerId = players[i].Id,
                        GameRank = correspondingPlayerRanks[i]
                    });
            }

            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
                {
                    GameDefinitionId = gameDefinitionId,
                    PlayerRanks = playerRanks,
                };

            return playedGameCreator.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);
        }

        private void CleanUpTestData()
        {
            using (NemeStatsDbContext nemeStatsDbContext = new NemeStatsDbContext())
            {
                CleanUpPlayerGameResults(nemeStatsDbContext);
                CleanUpPlayedGames(nemeStatsDbContext);
                nemeStatsDbContext.SaveChanges();
                
                CleanUpChampions(nemeStatsDbContext);
                CleanUpGameDefinitions(nemeStatsDbContext, testGameName);
                CleanUpGameDefinitions(nemeStatsDbContext, testGameName2);
                CleanUpGameDefinitions(nemeStatsDbContext, testGameNameForGameWithOtherGamingGroupId);
                CleanUpGameDefinitions(nemeStatsDbContext, testGameNameForAnotherGameWithOtherGamingGroupId);
                CleanUpGameDefinitions(nemeStatsDbContext, gameDefinitionWithNoChampionName);
                CleanUpPlayers(nemeStatsDbContext);
                nemeStatsDbContext.SaveChanges();

                CleanUpGamingGroup(testGamingGroup1Name, nemeStatsDbContext);
                CleanUpGamingGroup(testGamingGroup2Name, nemeStatsDbContext);
                CleanUpGamingGroup(testGamingGroup3Name, nemeStatsDbContext);
                nemeStatsDbContext.SaveChanges();

                CleanUpGamingGroupInvitation(testInviteeEmail1, nemeStatsDbContext);
                CleanUpGamingGroupInvitation(testInviteeEmail2, nemeStatsDbContext);
                nemeStatsDbContext.SaveChanges();

                CleanUpApplicationUser(testApplicationUserNameForUserWithDefaultGamingGroup, nemeStatsDbContext);
                CleanUpApplicationUser(testApplicationUserNameForUserWithOtherGamingGroup, nemeStatsDbContext);
                CleanUpApplicationUser(testApplicationUserNameForUserWithDefaultGamingGroupAndNoInvites, nemeStatsDbContext);
                CleanUpApplicationUser(testApplicationUserNameForUserWithThirdGamingGroup, nemeStatsDbContext);
                nemeStatsDbContext.SaveChanges();
            }
        }

        private void CleanUpGamingGroupInvitation(string inviteeEmail, NemeStatsDbContext nemeStatsDbContext)
        {
            GamingGroupInvitation invitation = (from gamingGroupInvitation in nemeStatsDbContext.GamingGroupInvitations
                                                       where gamingGroupInvitation.InviteeEmail == inviteeEmail
                                                select gamingGroupInvitation).FirstOrDefault();

            if (invitation != null)
            {
                try
                {
                    nemeStatsDbContext.GamingGroupInvitations.Remove(invitation);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayers(NemeStatsDbContext nemeStatsDbContext)
        {
            CleanUpNemeses(nemeStatsDbContext);

            CleanUpPlayerByPlayerName(testPlayer1Name, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayer2Name, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayer3Name, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayer4Name, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayer5Name, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayer6Name, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayer7Name, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayer8Name, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayer9UndefeatedWith5GamesName, nemeStatsDbContext);
            CleanUpPlayerByPlayerName(testPlayerWithNoPlayedGamesName, nemeStatsDbContext);
        }

        private void CleanUpChampions(NemeStatsDbContext nemeStatsDbContext)
        {
            List<int> gameDefinitionIdsToClearChampionId = (from gameDefinition in nemeStatsDbContext.GameDefinitions
                                                            where gameDefinition.Name == testGameName
                                                                || gameDefinition.Name == testGameName2
                                                                || gameDefinition.Name == testGameNameForGameWithOtherGamingGroupId
                                                                || gameDefinition.Name == testGameNameForAnotherGameWithOtherGamingGroupId
                                                            select gameDefinition.Id)
                                                            .ToList();

            championIdsToDelete = (from champion in nemeStatsDbContext.Champions
                                    where gameDefinitionIdsToClearChampionId.Contains(champion.GameDefinitionId)
                                    select champion.Id)
                                    .Distinct()
                                    .ToList();

            GameDefinition gameDefinitionNeedingChampionCleared;
            foreach (int gameDefinitionId in gameDefinitionIdsToClearChampionId)
            {
                gameDefinitionNeedingChampionCleared = nemeStatsDbContext.GameDefinitions.Find(gameDefinitionId);
                gameDefinitionNeedingChampionCleared.ChampionId = null;
            }
            nemeStatsDbContext.SaveChanges();

            Champion championToDelete;
            foreach (int championId in championIdsToDelete)
            {
                championToDelete = nemeStatsDbContext.Champions.Find(championId);
                nemeStatsDbContext.Champions.Remove(championToDelete);
            }
            nemeStatsDbContext.SaveChanges();
        }

        private void CleanUpNemeses(NemeStatsDbContext nemeStatsDbContext)
        {
            List<int> playerIdsToClearNemesisId = (from player in nemeStatsDbContext.Players
                                                   where player.Name == testPlayer1Name
                                                   || player.Name == testPlayer1Name
                                                   || player.Name == testPlayer2Name
                                                   || player.Name == testPlayer3Name
                                                   || player.Name == testPlayer3Name
                                                   || player.Name == testPlayer4Name
                                                   || player.Name == testPlayer5Name
                                                   || player.Name == testPlayer6Name
                                                   || player.Name == testPlayer7Name
                                                   || player.Name == testPlayer8Name
                                                   || player.Name == testPlayer9UndefeatedWith5GamesName
                                                   select player.Id)
                                                  .ToList();

            nemesisIdsToDelete = (from nemesis in nemeStatsDbContext.Nemeses
                                  where playerIdsToClearNemesisId.Contains(nemesis.NemesisPlayerId)
                                  select nemesis.Id)
                                            .Distinct()
                                            .ToList();

            Player playerNeedingNemesisCleared;
            foreach (int playerId in playerIdsToClearNemesisId)
            {
                playerNeedingNemesisCleared = nemeStatsDbContext.Players.Find(playerId);
                if (playerNeedingNemesisCleared.NemesisId != null)
                {
                    playerNeedingNemesisCleared.NemesisId = null;
                }
            }
            nemeStatsDbContext.SaveChanges();
            Nemesis nemesisToDelete;
            foreach (int nemesisId in nemesisIdsToDelete)
            {
                nemesisToDelete = nemeStatsDbContext.Nemeses.Find(nemesisId);
                nemeStatsDbContext.Nemeses.Remove(nemesisToDelete);
            }
            nemeStatsDbContext.SaveChanges();
        }

        private void CleanUpApplicationUser(string testApplicationUserName, NemeStatsDbContext nemeStatsDbContext)
        {
            ApplicationUser applicationUserToDelete = (from applicationUser in nemeStatsDbContext.Users
                                                       where applicationUser.UserName == testApplicationUserName
                                                       select applicationUser).FirstOrDefault();

            if (applicationUserToDelete != null)
            {
                try
                {
                    nemeStatsDbContext.Users.Remove(applicationUserToDelete);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpGamingGroup(string testGamingGroupName, NemeStatsDbContext nemeStatsDbContext)
        {
            GamingGroup gamingGroupToDelete = (from gamingGroup in nemeStatsDbContext.GamingGroups
                                               where gamingGroup.Name == testGamingGroupName
                                               select gamingGroup).FirstOrDefault();

            if (gamingGroupToDelete != null)
            {
                try
                {
                    nemeStatsDbContext.GamingGroups.Remove(gamingGroupToDelete);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayedGames(NemeStatsDbContext nemeStatsDbContext)
        {
            List<PlayedGame> playedGamesToDelete = (from playedGame in nemeStatsDbContext.PlayedGames
                                                    where playedGame.GameDefinition.Name == testGameName
                                                    select playedGame).ToList();

            foreach (PlayedGame playedGame in playedGamesToDelete)
            {
                try
                {
                    nemeStatsDbContext.PlayedGames.Remove(playedGame);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayerGameResults(NemeStatsDbContext nemeStatsDbContext)
        {
            List<PlayerGameResult> playerGameResultsToDelete = (from playerGameResult in nemeStatsDbContext.PlayerGameResults
                                                                where playerGameResult.PlayedGame.GameDefinition.Name == testGameName
                                                                select playerGameResult).ToList();

            foreach (PlayerGameResult playerGameResult in playerGameResultsToDelete)
            {
                try
                {
                    nemeStatsDbContext.PlayerGameResults.Remove(playerGameResult);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpGameDefinitions(NemeStatsDbContext nemeStatsDbContext, string gameDefinitionName)
        {
            List<GameDefinition> gameDefinitionsToDelete = (from game in nemeStatsDbContext.GameDefinitions
                                                            where game.Name == gameDefinitionName
                                                            select game).ToList();

            foreach (GameDefinition game in gameDefinitionsToDelete)
            {
                try
                {
                    nemeStatsDbContext.GameDefinitions.Remove(game);
                }
                catch (Exception) { }
            }
        }

        private static void CleanUpPlayerByPlayerName(string playerName, NemeStatsDbContext nemeStatsDbContext)
        {
            Player playerToDelete = nemeStatsDbContext.Players.FirstOrDefault(player => player.Name == playerName);

            if (playerToDelete != null)
            {
                try
                {
                    nemeStatsDbContext.Players.Remove(playerToDelete);
                    nemeStatsDbContext.SaveChanges();
                }
                catch (Exception) { }
            }
        }

        [OneTimeTearDown]
        public virtual void FixtureTearDown()
        {
            CleanUpTestData();
        }
    }
}
