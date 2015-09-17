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
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetPlayerDetailsTests
    {
        private RhinoAutoMocker<PlayerRetriever> autoMocker;
       
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
        private List<Champion> expectedChampionedGames;
        private List<PlayerVersusPlayerStatistics> expectedPlayerVersusPlayerStatistics; 
        private int gamingGroupId = 1985;
            
        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<PlayerRetriever>();
            autoMocker.PartialMockTheClassUnderTest();

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

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                                               .Return(players.AsQueryable());

            PlayerStatistics playerStatistics = new PlayerStatistics();

            autoMocker.ClassUnderTest.Expect(repo => repo.GetPlayerStatistics(Arg<int>.Is.Anything))
                .Repeat.Once()
                .Return(playerStatistics);

            autoMocker.ClassUnderTest.Expect(mock => mock.GetPlayerGameResultsWithPlayedGameAndGameDefinition(
                Arg<int>.Is.Anything, 
                Arg<int>.Is.Anything))
                            .Repeat.Once()
                            .Return(player.PlayerGameResults.ToList());

            this.expectedMinions = new List<Player>();
            autoMocker.ClassUnderTest.Expect(mock => mock.GetMinions(Arg<int>.Is.Anything))
                .Return(this.expectedMinions);

            expectedPlayerGameSummaries = new List<PlayerGameSummary>
            {
                new PlayerGameSummary()
            };
            autoMocker.Get<IPlayerRepository>().Expect(mock => mock.GetPlayerGameSummaries(Arg<int>.Is.Anything))
                                .Return(expectedPlayerGameSummaries);

            expectedChampionedGames = new List<Champion> { expectedChampion };
            autoMocker.ClassUnderTest.Expect(mock => mock.GetChampionedGames(Arg<int>.Is.Anything))
                .Return(expectedChampionedGames);

            expectedPlayerVersusPlayerStatistics = new List<PlayerVersusPlayerStatistics>();
            autoMocker.Get<IPlayerRepository>().Expect(mock => mock.GetPlayerVersusPlayersStatistics(Arg<int>.Is.Anything))
                      .Return(expectedPlayerVersusPlayerStatistics);

        }

        [Test]
        public void ItThrowsAKeyNotFoundExceptionIfThePlayerDoesNotExist()
        {
            const int invalidPlayerId = -1;
            string expectedMessage = string.Format(PlayerRetriever.EXCEPTION_MESSAGE_PLAYER_COULD_NOT_BE_FOUND, invalidPlayerId);
            Exception actualException = Assert.Throws<KeyNotFoundException>(
                                                                            () => autoMocker.ClassUnderTest.GetPlayerDetails(
                                                                                invalidPlayerId, 
                                                                                0));
            
            Assert.AreEqual(expectedMessage, actualException.Message);
        }

        [Test]
        public void ItSetsTheCurrentNemesis()
        {
            PlayerDetails playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedNemesis, playerDetails.CurrentNemesis);
        }

        [Test]
        public void ItSetsTheCurrentNemesisToANullNemesisIfThereIsNoNemesis()
        {
            PlayerDetails playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithNoNemesisEver.Id, numberOfRecentGames);

            Assert.True(playerDetails.CurrentNemesis is NullNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesis()
        {
            PlayerDetails playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedPriorNemesis, playerDetails.PreviousNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesisToANullNemesisIfThereIsNoPreviousNemesis()
        {
            PlayerDetails playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithOnlyACurrentNemesis.Id, numberOfRecentGames);

            Assert.True(playerDetails.PreviousNemesis is NullNemesis);
        }

        [Test]
        public void ItSetsThePlayersMinions()
        {
            PlayerDetails playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(this.expectedMinions, playerDetails.Minions);
        }

        [Test]
        public void ItSetsThePlayersGameSummaries()
        {
            PlayerDetails playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedPlayerGameSummaries, playerDetails.PlayerGameSummaries);
        }

        [Test]
        public void ItSetsThePlayersChampionedGames()
        {
            PlayerDetails playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithAChampionship.Id,
                numberOfRecentGames);

            Assert.That(playerDetails.ChampionedGames, Is.EqualTo(expectedChampionedGames));
        }

        [Test]
        public void ItSetsThePlayerVersusPlayersStatistics()
        {
            PlayerDetails playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithAChampionship.Id,
                numberOfRecentGames);

            Assert.That(playerDetails.PlayerVersusPlayersStatistics, Is.EqualTo(expectedPlayerVersusPlayerStatistics));
        }
    }
}
