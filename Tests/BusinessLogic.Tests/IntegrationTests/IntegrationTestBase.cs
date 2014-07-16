using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected List<PlayedGame> testPlayedGames = new List<PlayedGame>();
        protected GameDefinition testGameDefinition;
        protected GameDefinition testGameDefinitionWithOtherGamingGroupId;
        protected Player testPlayer1;
        protected string testPlayer1Name = "testPlayer1";
        protected Player testPlayer2;
        protected string testPlayer2Name = "testPlayer2";
        protected Player testPlayer3;
        protected string testPlayer3Name = "testPlayer3";
        protected Player testPlayer4;
        protected string testPlayer4Name = "testPlayer4";
        protected Player testPlayer5;
        protected string testPlayer5Name = "testPlayer5";
        protected Player testPlayer6;
        protected string testPlayer6Name = "testPlayer6";
        protected Player testPlayer7WithOtherGamingGroupId;
        protected string testPlayer7Name = "testPlayer7";
        protected string testGameName = "this is test game definition name";
        protected string testGameNameForGameWithOtherGamingGroupId = "this is test game definition name for game with other GamingGroupId";
        protected string testGameDescription = "this is a test game description 123abc";
        protected string testApplicationUserNameForUserWithDefaultGamingGroup = "username with default gaming group";
        protected UserContext testUserContextForUserWithDefaultGamingGroup;
        protected string testApplicationUserNameForUserWithOtherGamingGroup = "username with other gaming group";
        protected UserContext testUserContextForUserWithOtherGamingGroup;
        protected string testGamingGroup1Name = "this is test gaming group 1";
        protected string testGamingGroup2Name = "this is test gaming group 2";
        protected GamingGroup testGamingGroup;
        protected GamingGroup testOtherGamingGroup;
        protected string testInviteeEmail1 = "email1@email.com";
        protected GamingGroupInvitation testUnredeemedGamingGroupInvitation;
        protected string testInviteeEmail2 = "email2@email.com";
        protected GamingGroupInvitation testAlreadyRedeemedGamingGroupInvitation;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                CleanUpTestData();

                ApplicationUser testUserWithDefaultGamingGroup = SaveApplicationUser(
                    dbContext,
                    testApplicationUserNameForUserWithDefaultGamingGroup,
                    "a@mailinator.com");
                ApplicationUser testUserWithOtherGamingGroup = SaveApplicationUser(
                    dbContext,
                    testApplicationUserNameForUserWithOtherGamingGroup,
                    "b@mailinator.com");

                testGamingGroup = SaveGamingGroup(dbContext, testGamingGroup1Name, testUserWithDefaultGamingGroup.Id);
                testUserContextForUserWithDefaultGamingGroup = UpdateDatefaultGamingGroupOnUser(testUserWithDefaultGamingGroup, testGamingGroup, dbContext);
                testOtherGamingGroup = SaveGamingGroup(dbContext, testGamingGroup2Name, testUserWithOtherGamingGroup.Id);
                testUserContextForUserWithOtherGamingGroup = UpdateDatefaultGamingGroupOnUser(testUserWithOtherGamingGroup, testOtherGamingGroup, dbContext);

                testGameDefinition = SaveGameDefinition(dbContext, testGamingGroup.Id, testGameName);
                testGameDefinitionWithOtherGamingGroupId = SaveGameDefinition(dbContext, testOtherGamingGroup.Id, testGameNameForGameWithOtherGamingGroupId);
                SavePlayers(dbContext, testGamingGroup.Id, testOtherGamingGroup.Id);

                SaveGamingGroupInvitations(dbContext);

                CreatePlayedGames(dbContext);
            }
        }

        private void SaveGamingGroupInvitations(NemeStatsDbContext dbContext)
        {
            EntityFrameworkGamingGroupAccessGranter accessGranter = new EntityFrameworkGamingGroupAccessGranter(dbContext);
            testUnredeemedGamingGroupInvitation = accessGranter.GrantAccess(testInviteeEmail1, testUserContextForUserWithDefaultGamingGroup);

            testAlreadyRedeemedGamingGroupInvitation = accessGranter.GrantAccess(testInviteeEmail2, testUserContextForUserWithDefaultGamingGroup);
            //TODO simulating registration. Will need a separate method for this soon so this logic can be replaced
            testAlreadyRedeemedGamingGroupInvitation.DateRegistered = DateTime.UtcNow.AddDays(1);
            testAlreadyRedeemedGamingGroupInvitation.RegisteredUserId = testUserContextForUserWithOtherGamingGroup.ApplicationUserId;
            dbContext.GamingGroupInvitations.Add(testAlreadyRedeemedGamingGroupInvitation);
            dbContext.SaveChanges();
        }

        private UserContext UpdateDatefaultGamingGroupOnUser(ApplicationUser user, GamingGroup gamingGroup, NemeStatsDbContext dbContext)
        {
            user.CurrentGamingGroupId = gamingGroup.Id;
            dbContext.SaveChanges();

            return new UserContext()
            {
                ApplicationUserId = user.Id,
                GamingGroupId = gamingGroup.Id
            };
        }

        private void CreatePlayedGames(NemeStatsDbContext dbContext)
        {
            PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);

            List<Player> players = new List<Player>() { testPlayer1, testPlayer2 };
            List<int> playerRanks = new List<int>() { 1, 1 };
            PlayedGame playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int>() { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer1, testPlayer3, testPlayer2 };
            playerRanks = new List<int>() { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer3, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            //make player4 beat player 1 three times
            players = new List<Player>() { testPlayer4, testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int>() { 1, 2, 3, 4 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer4, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer4, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            //--make the inactive player5 beat player1 3 times
            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer5, testPlayer1 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            //make player 2 be the only one who beat player 5
            players = new List<Player>() { testPlayer2, testPlayer5 };
            playerRanks = new List<int>() { 1, 2 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithDefaultGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);

            //--create a game that has a different GamingGroupId
            players = new List<Player>() { testPlayer7WithOtherGamingGroupId };
            playerRanks = new List<int>() { 1 };
            playedGame = CreateTestPlayedGame(players, playerRanks, testUserContextForUserWithOtherGamingGroup, playedGameLogic);
            testPlayedGames.Add(playedGame);
        }

        private void SavePlayers(NemeStatsDbContext dbContext, int primaryGamingGroupId, int otherGamingGroupId)
        {
            testPlayer1 = new Player() { Name = testPlayer1Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer1);
            testPlayer2 = new Player() { Name = testPlayer2Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer2);
            testPlayer3 = new Player() { Name = testPlayer3Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer3);
            testPlayer4 = new Player() { Name = testPlayer4Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer4);
            testPlayer5 = new Player() { Name = testPlayer5Name, Active = false, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer5);
            testPlayer6 = new Player() { Name = testPlayer6Name, Active = true, GamingGroupId = primaryGamingGroupId };
            dbContext.Players.Add(testPlayer6);

            testPlayer7WithOtherGamingGroupId = new Player() { Name = testPlayer7Name, Active = true, GamingGroupId = otherGamingGroupId };
            dbContext.Players.Add(testPlayer7WithOtherGamingGroupId);

            dbContext.SaveChanges();
        }

        private GameDefinition SaveGameDefinition(NemeStatsDbContext dbContext, int gamingGroupId, string gameDefinitionName)
        {
            GameDefinition gameDefinition = new GameDefinition() { Name = testGameName, Description = testGameDescription, GamingGroupId = gamingGroupId };
            dbContext.GameDefinitions.Add(gameDefinition);
            dbContext.SaveChanges();

            return gameDefinition;
        }

        private ApplicationUser SaveApplicationUser(NemeStatsDbContext dbContext, string userName, string email)
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = email,
                UserName = userName,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };
            dbContext.Users.Add(applicationUser);
            dbContext.SaveChanges();

            return applicationUser;
        }

        private GamingGroup SaveGamingGroup(NemeStatsDbContext dbContext, string gamingGroupName, string owningUserId)
        {
            GamingGroup gamingGroup = new GamingGroup() { Name = gamingGroupName, OwningUserId = owningUserId };
            dbContext.GamingGroups.Add(gamingGroup);
            dbContext.SaveChanges();
            return gamingGroup;
        }

        private PlayedGame CreateTestPlayedGame(
            List<Player> players,
            List<int> correspondingPlayerRanks,
            UserContext userContext,
            PlayedGameRepository playedGameLogic)
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
                    GameDefinitionId = testGameDefinition.Id,
                    PlayerRanks = playerRanks,
                };

            return playedGameLogic.CreatePlayedGame(newlyCompletedGame, userContext);
        }

        private void CleanUpTestData()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                CleanUpPlayerGameResults(dbContext);
                CleanUpPlayedGames(dbContext);
                CleanUpGameDefinitions(dbContext, testGameName);
                CleanUpGameDefinitions(dbContext, testGameNameForGameWithOtherGamingGroupId);
                CleanUpPlayers(dbContext);
                CleanUpGamingGroup(testGamingGroup1Name, dbContext);
                CleanUpGamingGroup(testGamingGroup2Name, dbContext);
                CleanUpApplicationUser(testApplicationUserNameForUserWithDefaultGamingGroup, dbContext);
                CleanUpApplicationUser(testApplicationUserNameForUserWithOtherGamingGroup, dbContext);
                CleanUpGamingGroupInvitation(testInviteeEmail1, dbContext);
                CleanUpGamingGroupInvitation(testInviteeEmail2, dbContext);

                dbContext.SaveChanges();
            }
        }

        private void CleanUpGamingGroupInvitation(string inviteeEmail, NemeStatsDbContext dbContext)
        {
            GamingGroupInvitation invitation = (from gamingGroupInvitation in dbContext.GamingGroupInvitations
                                                       where gamingGroupInvitation.InviteeEmail == inviteeEmail
                                                select gamingGroupInvitation).FirstOrDefault();

            if (invitation != null)
            {
                try
                {
                    dbContext.GamingGroupInvitations.Remove(invitation);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayers(NemeStatsDbContext dbContext)
        {
            CleanUpPlayerByPlayerName(testPlayer1Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer2Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer3Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer4Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer5Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer6Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer7Name, dbContext);
        }

        private void CleanUpApplicationUser(string testApplicationUserName, NemeStatsDbContext dbContext)
        {
            ApplicationUser applicationUserToDelete = (from applicationUser in dbContext.Users
                                                       where applicationUser.UserName == testApplicationUserName
                                                       select applicationUser).FirstOrDefault();

            if (applicationUserToDelete != null)
            {
                try
                {
                    dbContext.Users.Remove(applicationUserToDelete);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpGamingGroup(string testGamingGroupName, NemeStatsDbContext dbContext)
        {
            GamingGroup gamingGroupToDelete = (from gamingGroup in dbContext.GamingGroups
                                               where gamingGroup.Name == testGamingGroupName
                                               select gamingGroup).FirstOrDefault();

            if (gamingGroupToDelete != null)
            {
                try
                {
                    dbContext.GamingGroups.Remove(gamingGroupToDelete);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayedGames(NemeStatsDbContext dbContext)
        {
            List<PlayedGame> playedGamesToDelete = (from playedGame in dbContext.PlayedGames
                                                    where playedGame.GameDefinition.Name == testGameName
                                                    select playedGame).ToList();

            foreach (PlayedGame playedGame in playedGamesToDelete)
            {
                try
                {
                    dbContext.PlayedGames.Remove(playedGame);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayerGameResults(NemeStatsDbContext dbContext)
        {
            List<PlayerGameResult> playerGameResultsToDelete = (from playerGameResult in dbContext.PlayerGameResults
                                                                where playerGameResult.PlayedGame.GameDefinition.Name == testGameName
                                                                select playerGameResult).ToList();

            foreach (PlayerGameResult playerGameResult in playerGameResultsToDelete)
            {
                try
                {
                    dbContext.PlayerGameResults.Remove(playerGameResult);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpGameDefinitions(NemeStatsDbContext dbContext, string gameDefinitionName)
        {
            List<GameDefinition> gameDefinitionsToDelete = (from game in dbContext.GameDefinitions
                                                            where game.Name == gameDefinitionName
                                                            select game).ToList();

            foreach (GameDefinition game in gameDefinitionsToDelete)
            {
                try
                {
                    dbContext.GameDefinitions.Remove(game);
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
                }
                catch (Exception) { }
            }
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            CleanUpTestData();
        }
    }
}
