using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetPlayerDetailsTests
    {
        private IDataContext dataContextMock;
        private IPlayerRepository playerRepositoryMock;
        private PlayerRetriever playerRetrieverPartialMock;
        private Player player;
        private Player playerWithOnlyACurrentNemesis;
        private Player playerWithNoNemesisEver;
        private Player playerWithAChampionship;
        private int numberOfRecentGames = 1;
        private Nemesis expectedNemesis;
        private Nemesis expectedPriorNemesis;
        private Champion expectedChampion;
        private List<Player> expectedMinions;
        private List<PlayerGameSummary> expectedPlayerGameSummaries;
        private List<Champion> expectedChampionships;
        private int gamingGroupId = 1985;
            
        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            playerRetrieverPartialMock = MockRepository.GeneratePartialMock<PlayerRetriever>(dataContextMock, playerRepositoryMock);

            expectedChampion = new Champion()
            {
                Id = 100,
                PlayerId = 101,
                Player = new Player()
            };
            expectedNemesis = new Nemesis()
            {
                Id = 155,
                NemesisPlayerId = 8888,
                NemesisPlayer = new Player()
            };
            expectedPriorNemesis = new Nemesis()
            {
                Id = 22222,
                NemesisPlayerId = 4444,
                NemesisPlayer = new Player()
            };
            Nemesis nemesisForPlayerWithOnlyACurrentNemesis = new Nemesis
            {
                Id = 33333,
                NemesisPlayer = new Player()
            };
            player = new Player()
            {
                Id = 1351,
                Name = "the player",
                PlayerGameResults = new List<PlayerGameResult>(),
                GamingGroup = new GamingGroup{ Id = gamingGroupId },
                GamingGroupId = gamingGroupId,
                Active = true,
                NemesisId = expectedNemesis.Id,
                Nemesis = expectedNemesis,
                PreviousNemesisId = expectedPriorNemesis.Id,
                PreviousNemesis = expectedPriorNemesis
            };
            playerWithNoNemesisEver = new Player()
            {
                Id = 161266939,
                GamingGroup = new GamingGroup { Id = gamingGroupId },
                GamingGroupId = gamingGroupId
            };
            playerWithOnlyACurrentNemesis = new Player()
            {
                Id = 888484,
                NemesisId = 7,
                Nemesis = nemesisForPlayerWithOnlyACurrentNemesis,
                GamingGroup = new GamingGroup { Id = gamingGroupId },
                GamingGroupId = gamingGroupId
            };
            playerWithAChampionship = new Player()
            {
                Id = 101,
                GamingGroup = new GamingGroup { Id = gamingGroupId }
            };

            List<Player> players = new List<Player>()
            {
                player,
                playerWithNoNemesisEver,
                playerWithOnlyACurrentNemesis,
                playerWithAChampionship
            };

            dataContextMock.Expect(mock => mock.GetQueryable<Player>())
                                               .Return(players.AsQueryable());

            PlayerStatistics playerStatistics = new PlayerStatistics();

            playerRetrieverPartialMock.Expect(repo => repo.GetPlayerStatistics(Arg<int>.Is.Anything))
                .Repeat.Once()
                .Return(playerStatistics);

            playerRetrieverPartialMock.Expect(mock => mock.GetPlayerGameResultsWithPlayedGameAndGameDefinition(
                Arg<int>.Is.Anything, 
                Arg<int>.Is.Anything))
                            .Repeat.Once()
                            .Return(player.PlayerGameResults.ToList());

            this.expectedMinions = new List<Player>();
            playerRetrieverPartialMock.Expect(mock => mock.GetMinions(Arg<int>.Is.Anything))
                .Return(this.expectedMinions);

            expectedPlayerGameSummaries = new List<PlayerGameSummary>
            {
                new PlayerGameSummary()
            };
            playerRepositoryMock.Expect(mock => mock.GetPlayerGameSummaries(Arg<int>.Is.Anything))
                                .Return(expectedPlayerGameSummaries);

            expectedChampionships = new List<Champion> { expectedChampion };
            playerRetrieverPartialMock.Expect(mock => mock.GetChampionships(Arg<int>.Is.Anything))
                .Return(expectedChampionships);

        }

        //TODO need tests for the transformation... which should probably be refactored into a different class

        [Test]
        public void ItThrowsAKeyNotFoundExceptionIfThePlayerDoesNotExist()
        {
            const int invalidPlayerId = -1;
            string expectedMessage = string.Format(PlayerRetriever.EXCEPTION_MESSAGE_PLAYER_COULD_NOT_BE_FOUND, invalidPlayerId);
            Exception actualException = Assert.Throws<KeyNotFoundException>(
                                                                            () => playerRetrieverPartialMock.GetPlayerDetails(
                                                                                invalidPlayerId, 
                                                                                0));
            
            Assert.AreEqual(expectedMessage, actualException.Message);
        }

        [Test]
        public void ItSetsTheCurrentNemesis()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedNemesis, playerDetails.CurrentNemesis);
        }

        [Test]
        public void ItSetsTheCurrentNemesisToANullNemesisIfThereIsNoNemesis()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(playerWithNoNemesisEver.Id, numberOfRecentGames);

            Assert.True(playerDetails.CurrentNemesis is NullNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesis()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedPriorNemesis, playerDetails.PreviousNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesisToANullNemesisIfThereIsNoPreviousNemesis()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(playerWithOnlyACurrentNemesis.Id, numberOfRecentGames);

            Assert.True(playerDetails.PreviousNemesis is NullNemesis);
        }

        [Test]
        public void ItSetsThePlayersMinions()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(this.expectedMinions, playerDetails.Minions);
        }

        [Test]
        public void ItSetsThePlayersGameSummaries()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedPlayerGameSummaries, playerDetails.PlayerGameSummaries);
        }

        [Test]
        public void ItSetsThePlayersChampionships()
        {
            PlayerDetails playerDetails = playerRetrieverPartialMock.GetPlayerDetails(playerWithAChampionship.Id,
                numberOfRecentGames);

            Assert.That(playerDetails.Championships, Is.EqualTo(expectedChampionships));
        }
    }
}
