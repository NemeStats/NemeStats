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
using BusinessLogic.Exceptions;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Points;
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
        private readonly int numberOfRecentGames = 1;
        private Nemesis expectedNemesis;
        private Nemesis expectedPriorNemesis;
        private Champion expectedChampion;
        private GameDefinition expectedFormerChampionGame;
        private PlayerStatistics expectedPlayerStatistics;
        private List<Player> expectedMinions;
        private List<PlayerGameSummary> expectedPlayerGameSummaries;
        private List<Champion> expectedChampionedGames;
        private List<GameDefinition> expectedFormerChampionedGames;
        private List<PlayerVersusPlayerStatistics> expectedPlayerVersusPlayerStatistics; 
        private readonly int gamingGroupId = 1985;
        private readonly int expectedLongestWinningStreak = 93;
            
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
            var nemesisForPlayerWithOnlyACurrentNemesis = new Nemesis
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
            expectedFormerChampionGame = new GameDefinition
            {
                Id = 111,
                PreviousChampionId = player.Id,
                GamingGroupId = gamingGroupId
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

            var players = new List<Player>()
            {
                player,
                playerWithNoNemesisEver,
                playerWithOnlyACurrentNemesis,
                playerWithAChampionship
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                                               .Return(players.AsQueryable());

            expectedPlayerStatistics = new PlayerStatistics
            {
                NemePointsSummary = new NemePointsSummary(1, 2, 4)
            };

            autoMocker.ClassUnderTest.Expect(repo => repo.GetPlayerStatistics(Arg<int>.Is.Anything))
                .Repeat.Once()
                .Return(expectedPlayerStatistics);

            autoMocker.ClassUnderTest.Expect(mock => mock.GetPlayerGameResultsWithPlayedGameAndGameDefinition(
                Arg<int>.Is.Anything, 
                Arg<int>.Is.Anything))
                            .Repeat.Once()
                            .Return(player.PlayerGameResults.ToList());

            expectedMinions = new List<Player>();
            autoMocker.ClassUnderTest.Expect(mock => mock.GetMinions(Arg<int>.Is.Anything))
                .Return(expectedMinions);

            expectedPlayerGameSummaries = new List<PlayerGameSummary>
            {
                new PlayerGameSummary()
            };
            autoMocker.Get<IPlayerRepository>().Expect(mock => mock.GetPlayerGameSummaries(Arg<int>.Is.Anything, Arg<IDataContext>.Is.Anything))
                                .Return(expectedPlayerGameSummaries);

            expectedChampionedGames = new List<Champion> { expectedChampion };
            autoMocker.ClassUnderTest.Expect(mock => mock.GetChampionedGames(Arg<int>.Is.Anything))
                .Return(expectedChampionedGames);

            expectedFormerChampionedGames = new List<GameDefinition> { expectedFormerChampionGame };
            autoMocker.ClassUnderTest.Expect(mock => mock.GetFormerChampionedGames(Arg<int>.Is.Anything))
                .Return(expectedFormerChampionedGames);

            expectedPlayerVersusPlayerStatistics = new List<PlayerVersusPlayerStatistics>();
            autoMocker.Get<IPlayerRepository>().Expect(mock => mock.GetPlayerVersusPlayersStatistics(Arg<int>.Is.Anything, Arg<IDataContext>.Is.Anything))
                      .Return(expectedPlayerVersusPlayerStatistics);

            autoMocker.Get<IPlayerRepository>().Expect(mock => mock.GetLongestWinningStreak(Arg<int>.Is.Equal(player.Id), Arg<IDataContext>.Is.Anything)).Return(expectedLongestWinningStreak);

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>()).Return(new List<PlayerAchievement>().AsQueryable());

        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfThePlayerDoesNotExist()
        {
            const int invalidPlayerId = -1;
            var expectedException = new EntityDoesNotExistException(typeof(Player), invalidPlayerId);
            Exception actualException = Assert.Throws<EntityDoesNotExistException>(
                                                                            () => autoMocker.ClassUnderTest.GetPlayerDetails(
                                                                                invalidPlayerId, 
                                                                                0));
            
            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItSetsTheCurrentNemesis()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedNemesis, playerDetails.CurrentNemesis);
        }

        [Test]
        public void ItSetsTheCurrentNemesisToANullNemesisIfThereIsNoNemesis()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithNoNemesisEver.Id, numberOfRecentGames);

            Assert.True(playerDetails.CurrentNemesis is NullNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesis()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedPriorNemesis, playerDetails.PreviousNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesisToANullNemesisIfThereIsNoPreviousNemesis()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithOnlyACurrentNemesis.Id, numberOfRecentGames);

            Assert.True(playerDetails.PreviousNemesis is NullNemesis);
        }

        [Test]
        public void ItSetsThePlayersMinions()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedMinions, playerDetails.Minions);
        }

        [Test]
        public void ItSetsThePlayersGameSummaries()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.AreSame(expectedPlayerGameSummaries, playerDetails.PlayerGameSummaries);
        }

        [Test]
        public void ItSetsThePlayersChampionedGames()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithAChampionship.Id,
                numberOfRecentGames);

            Assert.That(playerDetails.ChampionedGames, Is.EqualTo(expectedChampionedGames));
        }

        [Test]
        public void ItSetsThePlayersFormerChampionedGames()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithAChampionship.Id,
                numberOfRecentGames);

            Assert.That(playerDetails.FormerChampionedGames, Is.EqualTo(expectedFormerChampionedGames));
        }

        [Test]
        public void ItSetsThePlayerVersusPlayersStatistics()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(playerWithAChampionship.Id,
                numberOfRecentGames);

            Assert.That(playerDetails.PlayerVersusPlayersStatistics, Is.EqualTo(expectedPlayerVersusPlayerStatistics));
        }

        [Test]
        public void ItSetsTheLongestWinningStreak()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.That(playerDetails.LongestWinningStreak, Is.EqualTo(expectedLongestWinningStreak)); 
        }

        [Test]
        public void ItSetsTheNemePointsSummary()
        {
            var playerDetails = autoMocker.ClassUnderTest.GetPlayerDetails(player.Id, numberOfRecentGames);

            Assert.That(playerDetails.NemePointsSummary, Is.EqualTo(expectedPlayerStatistics.NemePointsSummary));
        }


    }
}
