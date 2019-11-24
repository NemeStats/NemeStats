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
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Service;
using BusinessLogic.Logic.BoardGameGeek;
using RollbarSharp;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture, Category("Integration")]
    public class IntegrationTestBase : IntegrationTestIoCBase
    {
        public const int BOARD_GAME_GEEK_ID_FOR_RACE_FOR_THE_GALAXY = 28143;

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
        protected BoardGameGeekGameDefinition testBoardGameGeekGameDefinition;
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

        private IDataContext _dataContext;
        private NemeStatsDbContext _nemeStatsDbContext;

        [OneTimeSetUp]
        public virtual void FixtureSetUp()
        {
            //create a stub for this only since we don't want the slowdown of all of the universal analytics event tracking
            eventTrackerStub = MockRepository.GenerateStub<IEventTracker>();
            eventTrackerStub.Expect(stub => stub.TrackEvent(Arg<IUniversalAnalyticsEvent>.Is.Anything))
                .Repeat.Any();
             
            playedGameTracker = new UniversalAnalyticsNemeStatsEventTracker(eventTrackerStub, eventFactory);

            _dataContext = GetInstance<IDataContext>();
            _nemeStatsDbContext = GetInstance<NemeStatsDbContext>();

            CleanUpTestData();

            testUserWithDefaultGamingGroup = SaveApplicationUser(testApplicationUserNameForUserWithDefaultGamingGroup, "a@mailinator.com");
            testUserWithDefaultGamingGroupAndNoInvites = SaveApplicationUser(testApplicationUserNameForUserWithDefaultGamingGroupAndNoInvites, "a2@mailinator.com");
            testUserWithOtherGamingGroup = SaveApplicationUser(testApplicationUserNameForUserWithOtherGamingGroup, "b@mailinator.com");
            testUserWithThirdGamingGroup = SaveApplicationUser(testApplicationUserNameForUserWithThirdGamingGroup, "c@mailinator.com");

            testGamingGroup = SaveGamingGroup(testGamingGroup1Name, testUserWithDefaultGamingGroup);
            testUserWithDefaultGamingGroup = UpdateDatefaultGamingGroupOnUser(testUserWithDefaultGamingGroup, testGamingGroup);
            testOtherGamingGroup = SaveGamingGroup(testGamingGroup2Name, testUserWithOtherGamingGroup);
            testUserWithOtherGamingGroup = UpdateDatefaultGamingGroupOnUser(testUserWithOtherGamingGroup, testOtherGamingGroup);
            testThirdGamingGroup = SaveGamingGroup(testGamingGroup3Name, testUserWithThirdGamingGroup);
            testUserWithThirdGamingGroup = UpdateDatefaultGamingGroupOnUser(testUserWithThirdGamingGroup, testThirdGamingGroup);

            testBoardGameGeekGameDefinition = SaveBoardGameGeekGameDefinition();

            testGameDefinition = SaveGameDefinition(testGamingGroup.Id, testGameName, testBoardGameGeekGameDefinition.Id);
            testGameDefinition2 = SaveGameDefinition(testGamingGroup.Id, testGameName2);
            testGameDefinitionWithOtherGamingGroupId = SaveGameDefinition(testOtherGamingGroup.Id, testGameNameForGameWithOtherGamingGroupId);
            gameDefinitionWithNoChampion = SaveGameDefinition(testThirdGamingGroup.Id,
                gameDefinitionWithNoChampionName);
            anotherTestGameDefinitionWithOtherGamingGroupId = SaveGameDefinition(testOtherGamingGroup.Id, testGameNameForAnotherGameWithOtherGamingGroupId);
            SavePlayers(testGamingGroup.Id, testOtherGamingGroup.Id);

            CreatePlayedGames();
        }

        [OneTimeTearDown]
        public virtual void TestFixtureTearDown()
        {
            try
            {
                _nemeStatsDbContext.Dispose();
            }
            catch (Exception)
            {
            }

            try
            {
                _dataContext.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private ApplicationUser UpdateDatefaultGamingGroupOnUser(ApplicationUser user, GamingGroup gamingGroup)
        {
            user.CurrentGamingGroupId = gamingGroup.Id;
            _dataContext.CommitAllChanges();

            return user;
        }

        private BoardGameGeekGameDefinition SaveBoardGameGeekGameDefinition()
        {
            var boardGameGeekDefinitionCreator = new BoardGameGeekGameDefinitionCreator(_dataContext, new BoardGameGeekClient(new ApiDownloaderService(), MockRepository.GenerateMock<IRollbarClient>()));
            return boardGameGeekDefinitionCreator.CreateBoardGameGeekGameDefinition(BOARD_GAME_GEEK_ID_FOR_RACE_FOR_THE_GALAXY);
        }

        private void CreatePlayedGames()
        {
            var createPlayedGameComponent = GetInstance<CreatePlayedGameComponent>();

            List<Player> players = new List<Player> { testPlayer1, testPlayer2 };
            List<int> playerRanks = new List<int> { 1, 1 };
            PlayedGame playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int> { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer1, testPlayer3, testPlayer2 };
            playerRanks = new List<int> { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer3, testPlayer1 };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            //make player4 beat player 1 three times
            players = new List<Player> { testPlayer4, testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int> { 1, 2, 3, 4 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer4, testPlayer1 };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer4, testPlayer1 };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            //--make the inactive player5 beat player1 3 times
            players = new List<Player> { testPlayer5, testPlayer1 };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer5, testPlayer1 };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer5, testPlayer1 };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            //make player 2 be the only one who beat player 5
            players = new List<Player> { testPlayer2, testPlayer5 };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            //--create games that have a different GamingGroupId and testPlayer7 being the champion
            players = new List<Player> { testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int> { 1 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer7WithOtherGamingGroupId, testPlayer8WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer7WithOtherGamingGroupId, testPlayer8WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer7WithOtherGamingGroupId, testPlayer8WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer7WithOtherGamingGroupId, testPlayer8WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer8WithOtherGamingGroupId, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext);
            testPlayedGames.Add(playedGame);

            //--this played game is future dated to a valid date on the Thai Buddhist calendar. Board Game Stats is allowing games like this to pass through... Tried to pick two players that wouldn't hurt
            // other integration tests
            players = new List<Player> { testPlayer2, testPlayer5 };
            playerRanks = new List<int> { 1, 2 };
            playedGame = CreateTestPlayedGame(testGameDefinition.Id, players, playerRanks, testUserWithDefaultGamingGroup, createPlayedGameComponent, _dataContext, datePlayed: DateTime.UtcNow.AddYears(543));
            testPlayedGames.Add(playedGame);

            players = new List<Player> { testPlayer9UndefeatedWith5Games, testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int> { 1, 2 };
            //--this last one should pause to finish all of the post-update processing
            playedGame = CreateTestPlayedGame(anotherTestGameDefinitionWithOtherGamingGroupId.Id, players, playerRanks, testUserWithOtherGamingGroup, createPlayedGameComponent, _dataContext, true);
            testPlayedGames.Add(playedGame);
        }

        private void SavePlayers(int primaryGamingGroupId, int otherGamingGroupId)
        {
            testPlayer1 = new Player { Name = testPlayer1Name, Active = true, GamingGroupId = primaryGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayer1);
            testPlayer2 = new Player { Name = testPlayer2Name, Active = true, GamingGroupId = primaryGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayer2);
            testPlayer3 = new Player { Name = testPlayer3Name, Active = true, GamingGroupId = primaryGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayer3);
            testPlayer4 = new Player { Name = testPlayer4Name, Active = true, GamingGroupId = primaryGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayer4);
            testPlayer5 = new Player { Name = testPlayer5Name, Active = false, GamingGroupId = primaryGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayer5);
            testPlayer6 = new Player { Name = testPlayer6Name, Active = true, GamingGroupId = primaryGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayer6);

            testPlayer7WithOtherGamingGroupId = new Player { Name = testPlayer7Name, Active = true, GamingGroupId = otherGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayer7WithOtherGamingGroupId);
            testPlayer8WithOtherGamingGroupId = new Player { Name = testPlayer8Name, Active = true, GamingGroupId = otherGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayer8WithOtherGamingGroupId);
            testPlayer9UndefeatedWith5Games = new Player { Name = testPlayer9UndefeatedWith5GamesName, Active = false, GamingGroupId =  otherGamingGroupId};
            _nemeStatsDbContext.Players.Add(testPlayer9UndefeatedWith5Games);

            testPlayerWithNoPlayedGames = new Player { Name = testPlayerWithNoPlayedGamesName, Active = true, GamingGroupId = primaryGamingGroupId };
            _nemeStatsDbContext.Players.Add(testPlayerWithNoPlayedGames);

            _nemeStatsDbContext.SaveChanges();
        }

        private GameDefinition SaveGameDefinition(int gamingGroupId, string gameDefinitionName, int? boardGameGeekGameDefinitionid = null)
        {
            GameDefinition gameDefinition = new GameDefinition
            {
                Name = gameDefinitionName,
                Description = testGameDescription,
                GamingGroupId = gamingGroupId,
                BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionid
            };
            _nemeStatsDbContext.GameDefinitions.Add(gameDefinition);
            _nemeStatsDbContext.SaveChanges();

            return gameDefinition;
        }

        private ApplicationUser SaveApplicationUser(string userName, string email)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = email,
                UserName = userName,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                AnonymousClientId = new Guid().ToString()
            };
            _nemeStatsDbContext.Users.Add(applicationUser);
            _nemeStatsDbContext.SaveChanges();

            return applicationUser;
        }

        private GamingGroup SaveGamingGroup(string gamingGroupName, ApplicationUser owningUser)
        {
            GamingGroup gamingGroup = new GamingGroup { Name = gamingGroupName, OwningUserId = owningUser.Id };
            _dataContext.Save(gamingGroup, owningUser);
            _dataContext.CommitAllChanges();

            return gamingGroup;
        }

        private PlayedGame CreateTestPlayedGame(
            int gameDefinitionId,
            List<Player> players,
            List<int> correspondingPlayerRanks,
            ApplicationUser currentUser,
            CreatePlayedGameComponent createdPlayedGameComponent,
            IDataContext _dataContext,
            bool waitForAllPostSaveEventHandlingToFinish = false,
            DateTime? datePlayed = null)
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();

            for (int i = 0; i < players.Count; i++)
            {
                playerRanks.Add(new PlayerRank
                {
                        PlayerId = players[i].Id,
                        GameRank = correspondingPlayerRanks[i]
                    });
            }

            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame
            {
                    GameDefinitionId = gameDefinitionId,
                    PlayerRanks = playerRanks,
                    TransactionSource = TransactionSource.WebApplication
            };

            if (datePlayed.HasValue)
            {
                newlyCompletedGame.DatePlayed = datePlayed.Value;
            }

            var playedGame = createdPlayedGameComponent.ExecuteTransaction(newlyCompletedGame, currentUser, _dataContext);

            if (waitForAllPostSaveEventHandlingToFinish)
            {
                createdPlayedGameComponent.PostSaveTask.Wait();
            }

            return playedGame;
        }

        private void CleanUpTestData()
        {
            CleanUpPlayerGameResults();
            CleanUpPlayedGames();
            _nemeStatsDbContext.SaveChanges();
                
            CleanUpChampions();
            CleanUpGameDefinitions(testGameName);
            CleanUpGameDefinitions(testGameName2);
            CleanUpGameDefinitions(testGameNameForGameWithOtherGamingGroupId);
            CleanUpGameDefinitions(testGameNameForAnotherGameWithOtherGamingGroupId);
            CleanUpGameDefinitions(gameDefinitionWithNoChampionName);
            CleanUpBoardGameGeekGameDefinitions(testBoardGameGeekGameDefinition);
            CleanUpPlayers();
            _nemeStatsDbContext.SaveChanges();

            CleanUpGamingGroup(testGamingGroup1Name);
            CleanUpGamingGroup(testGamingGroup2Name);
            CleanUpGamingGroup(testGamingGroup3Name);
            _nemeStatsDbContext.SaveChanges();

            CleanUpGamingGroupInvitation(testInviteeEmail1);
            CleanUpGamingGroupInvitation(testInviteeEmail2);
            _nemeStatsDbContext.SaveChanges();

            CleanUpApplicationUser(testApplicationUserNameForUserWithDefaultGamingGroup);
            CleanUpApplicationUser(testApplicationUserNameForUserWithOtherGamingGroup);
            CleanUpApplicationUser(testApplicationUserNameForUserWithDefaultGamingGroupAndNoInvites);
            CleanUpApplicationUser(testApplicationUserNameForUserWithThirdGamingGroup);
            _nemeStatsDbContext.SaveChanges();
        }

        private void CleanUpGamingGroupInvitation(string inviteeEmail)
        {
            GamingGroupInvitation invitation = (from gamingGroupInvitation in _nemeStatsDbContext.GamingGroupInvitations
                                                       where gamingGroupInvitation.InviteeEmail == inviteeEmail
                                                select gamingGroupInvitation).FirstOrDefault();

            if (invitation != null)
            {
                try
                {
                    _nemeStatsDbContext.GamingGroupInvitations.Remove(invitation);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayers()
        {
            CleanUpNemeses();

            CleanUpPlayerByPlayerName(testPlayer1Name);
            CleanUpPlayerByPlayerName(testPlayer2Name);
            CleanUpPlayerByPlayerName(testPlayer3Name);
            CleanUpPlayerByPlayerName(testPlayer4Name);
            CleanUpPlayerByPlayerName(testPlayer5Name);
            CleanUpPlayerByPlayerName(testPlayer6Name);
            CleanUpPlayerByPlayerName(testPlayer7Name);
            CleanUpPlayerByPlayerName(testPlayer8Name);
            CleanUpPlayerByPlayerName(testPlayer9UndefeatedWith5GamesName);
            CleanUpPlayerByPlayerName(testPlayerWithNoPlayedGamesName);
        }

        private void CleanUpChampions()
        {
            List<int> gameDefinitionIdsToClearChampionId = (from gameDefinition in _nemeStatsDbContext.GameDefinitions
                                                            where gameDefinition.Name == testGameName
                                                                || gameDefinition.Name == testGameName2
                                                                || gameDefinition.Name == testGameNameForGameWithOtherGamingGroupId
                                                                || gameDefinition.Name == testGameNameForAnotherGameWithOtherGamingGroupId
                                                                || gameDefinition.Name == gameDefinitionWithNoChampionName
                                                            select gameDefinition.Id)
                                                            .ToList();

            championIdsToDelete = (from champion in _nemeStatsDbContext.Champions
                                    where gameDefinitionIdsToClearChampionId.Contains(champion.GameDefinitionId)
                                    select champion.Id)
                                    .Distinct()
                                    .ToList();

            GameDefinition gameDefinitionNeedingChampionCleared;
            foreach (int gameDefinitionId in gameDefinitionIdsToClearChampionId)
            {
                gameDefinitionNeedingChampionCleared = _nemeStatsDbContext.GameDefinitions.Find(gameDefinitionId);
                gameDefinitionNeedingChampionCleared.ChampionId = null;
            }
            _nemeStatsDbContext.SaveChanges();

            Champion championToDelete;
            foreach (int championId in championIdsToDelete)
            {
                championToDelete = _nemeStatsDbContext.Champions.Find(championId);
                _nemeStatsDbContext.Champions.Remove(championToDelete);
            }
            _nemeStatsDbContext.SaveChanges();
        }

        private void CleanUpNemeses()
        {
            List<int> playerIdsToClearNemesisId = (from player in _nemeStatsDbContext.Players
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

            nemesisIdsToDelete = (from nemesis in _nemeStatsDbContext.Nemeses
                                  where playerIdsToClearNemesisId.Contains(nemesis.NemesisPlayerId)
                                  select nemesis.Id)
                                            .Distinct()
                                            .ToList();

            Player playerNeedingNemesisCleared;
            foreach (int playerId in playerIdsToClearNemesisId)
            {
                playerNeedingNemesisCleared = _nemeStatsDbContext.Players.Find(playerId);
                if (playerNeedingNemesisCleared.NemesisId != null)
                {
                    playerNeedingNemesisCleared.NemesisId = null;
                }
            }
            _nemeStatsDbContext.SaveChanges();
            Nemesis nemesisToDelete;
            foreach (int nemesisId in nemesisIdsToDelete)
            {
                nemesisToDelete = _nemeStatsDbContext.Nemeses.Find(nemesisId);
                _nemeStatsDbContext.Nemeses.Remove(nemesisToDelete);
            }
            _nemeStatsDbContext.SaveChanges();
        }

        private void CleanUpApplicationUser(string testApplicationUserName)
        {
            ApplicationUser applicationUserToDelete = (from applicationUser in _nemeStatsDbContext.Users
                                                       where applicationUser.UserName == testApplicationUserName
                                                       select applicationUser).FirstOrDefault();

            if (applicationUserToDelete != null)
            {
                try
                {
                    _nemeStatsDbContext.Users.Remove(applicationUserToDelete);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpGamingGroup(string testGamingGroupName)
        {
            GamingGroup gamingGroupToDelete = (from gamingGroup in _nemeStatsDbContext.GamingGroups
                                               where gamingGroup.Name == testGamingGroupName
                                               select gamingGroup).FirstOrDefault();

            if (gamingGroupToDelete != null)
            {
                try
                {
                    _nemeStatsDbContext.GamingGroups.Remove(gamingGroupToDelete);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayedGames()
        {
            List<PlayedGame> playedGamesToDelete = (from playedGame in _nemeStatsDbContext.PlayedGames
                                                    where playedGame.GameDefinition.Name == testGameName
                                                    select playedGame).ToList();

            foreach (PlayedGame playedGame in playedGamesToDelete)
            {
                try
                {
                    _nemeStatsDbContext.PlayedGames.Remove(playedGame);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayerGameResults()
        {
            List<PlayerGameResult> playerGameResultsToDelete = (from playerGameResult in _nemeStatsDbContext.PlayerGameResults
                                                                where playerGameResult.PlayedGame.GameDefinition.Name == testGameName
                                                                select playerGameResult).ToList();

            foreach (PlayerGameResult playerGameResult in playerGameResultsToDelete)
            {
                try
                {
                    _nemeStatsDbContext.PlayerGameResults.Remove(playerGameResult);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpGameDefinitions(string gameDefinitionName)
        {
            List<GameDefinition> gameDefinitionsToDelete = (from game in _nemeStatsDbContext.GameDefinitions
                                                            where game.Name == gameDefinitionName
                                                            select game).ToList();

            foreach (GameDefinition game in gameDefinitionsToDelete)
            {
                try
                {
                    _nemeStatsDbContext.GameDefinitions.Remove(game);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpBoardGameGeekGameDefinitions(BoardGameGeekGameDefinition boardGameGeekGameDefinition)
        {
            try
            {
                _nemeStatsDbContext.BoardGameGeekGameDefinitions.Remove(boardGameGeekGameDefinition);
            }
            catch (Exception)
            {
            }
        }

        private void CleanUpPlayerByPlayerName(string playerName)
        {
            Player playerToDelete = _nemeStatsDbContext.Players.FirstOrDefault(player => player.Name == playerName);

            if (playerToDelete != null)
            {
                //--can never have more than one of these so firstOrDefault should be fine
                var championedGamingGroup =
                    _nemeStatsDbContext.GamingGroups.FirstOrDefault(gamingGroup => gamingGroup.GamingGroupChampionPlayerId == playerToDelete.Id);
                if (championedGamingGroup != null)
                {
                    championedGamingGroup.GamingGroupChampionPlayerId = null;
                }

                try
                {
                    _nemeStatsDbContext.Players.Remove(playerToDelete);
                    _nemeStatsDbContext.SaveChanges();
                }
                catch (Exception) { }
            }
        }
    }
}
