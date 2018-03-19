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
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Points;
using BusinessLogic.Paging;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using PagedList;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetPlayerDetailsTests
    {
        private RhinoAutoMocker<PlayerRetriever> _autoMocker;
       
        private Player _player;
        private Player _playerWithOnlyACurrentNemesis;
        private Player _playerWithNoNemesisEver;
        private Player _playerWithAChampionship;
        private readonly int _numberOfRecentGames = 1;
        private Nemesis _expectedNemesis;
        private Nemesis _expectedPriorNemesis;
        private Champion _expectedChampion;
        private GameDefinition _expectedFormerChampionGame;
        private PlayerStatistics _expectedPlayerStatistics;
        private List<Player> _expectedMinions;
        private List<PlayerGameSummary> _expectedPlayerGameSummaries;
        private List<Champion> _expectedChampionedGames;
        private List<GameDefinition> _expectedFormerChampionedGames;
        private List<PlayerVersusPlayerStatistics> _expectedPlayerVersusPlayerStatistics;
        private IPagedList<PlayerAchievementWinner> _expectedPlayerAchievementWinnersPageableList;
        private readonly int _gamingGroupId = 1985;
        private readonly int _expectedLongestWinningStreak = 93;
            
        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<PlayerRetriever>();
            _autoMocker.PartialMockTheClassUnderTest();
          
            _expectedChampion = new Champion()
            {
                Id = 100,
                PlayerId = 101,
                Player = new Player()
            };
            _expectedNemesis = new Nemesis()
            {
                Id = 155,
                NemesisPlayerId = 8888,
                NemesisPlayer = new Player()
            };
            _expectedPriorNemesis = new Nemesis()
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
            _player = new Player()
            {
                Id = 1351,
                Name = "the player",
                PlayerGameResults = new List<PlayerGameResult>(),
                GamingGroup = new GamingGroup{ Id = _gamingGroupId, Name = "some gaming group name" },
                GamingGroupId = _gamingGroupId,
                Active = true,
                NemesisId = _expectedNemesis.Id,
                Nemesis = _expectedNemesis,
                PreviousNemesisId = _expectedPriorNemesis.Id,
                PreviousNemesis = _expectedPriorNemesis,
                ApplicationUserId = "some user id"
            };
            _expectedFormerChampionGame = new GameDefinition
            {
                Id = 111,
                PreviousChampionId = _player.Id,
                GamingGroupId = _gamingGroupId
            };
            _playerWithNoNemesisEver = new Player()
            {
                Id = 161266939,
                GamingGroup = new GamingGroup { Id = _gamingGroupId },
                GamingGroupId = _gamingGroupId
            };
            _playerWithOnlyACurrentNemesis = new Player()
            {
                Id = 888484,
                NemesisId = 7,
                Nemesis = nemesisForPlayerWithOnlyACurrentNemesis,
                GamingGroup = new GamingGroup { Id = _gamingGroupId },
                GamingGroupId = _gamingGroupId
            };
            _playerWithAChampionship = new Player()
            {
                Id = 101,
                GamingGroup = new GamingGroup { Id = _gamingGroupId }
            };

            var players = new List<Player>()
            {
                _player,
                _playerWithNoNemesisEver,
                _playerWithOnlyACurrentNemesis,
                _playerWithAChampionship
            };

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                                               .Return(players.AsQueryable());

            _expectedPlayerStatistics = new PlayerStatistics
            {
                NemePointsSummary = new NemePointsSummary(1, 2, 4)
            };

            _autoMocker.ClassUnderTest.Expect(repo => repo.GetPlayerStatistics(Arg<int>.Is.Anything))
                .Repeat.Once()
                .Return(_expectedPlayerStatistics);

            _autoMocker.ClassUnderTest.Expect(mock => mock.GetPlayerGameResultsWithPlayedGameAndGameDefinition(
                Arg<int>.Is.Anything, 
                Arg<int>.Is.Anything))
                            .Repeat.Once()
                            .Return(_player.PlayerGameResults.ToList());

            _expectedMinions = new List<Player>();
            _autoMocker.ClassUnderTest.Expect(mock => mock.GetMinions(Arg<int>.Is.Anything))
                .Return(_expectedMinions);

            _expectedPlayerGameSummaries = new List<PlayerGameSummary>
            {
                new PlayerGameSummary()
            };
            _autoMocker.Get<IPlayerRepository>().Expect(mock => mock.GetPlayerGameSummaries(Arg<int>.Is.Anything, Arg<IDataContext>.Is.Anything))
                                .Return(_expectedPlayerGameSummaries);

            _expectedChampionedGames = new List<Champion> { _expectedChampion };
            _autoMocker.ClassUnderTest.Expect(mock => mock.GetChampionedGames(Arg<int>.Is.Anything))
                .Return(_expectedChampionedGames);

            _expectedFormerChampionedGames = new List<GameDefinition> { _expectedFormerChampionGame };
            _autoMocker.ClassUnderTest.Expect(mock => mock.GetFormerChampionedGames(Arg<int>.Is.Anything))
                .Return(_expectedFormerChampionedGames);

            _expectedPlayerVersusPlayerStatistics = new List<PlayerVersusPlayerStatistics>();
            _autoMocker.Get<IPlayerRepository>().Expect(mock => mock.GetPlayerVersusPlayersStatistics(Arg<int>.Is.Anything, Arg<IDataContext>.Is.Anything))
                      .Return(_expectedPlayerVersusPlayerStatistics);

            _autoMocker.Get<IPlayerRepository>().Expect(mock => mock.GetLongestWinningStreak(Arg<int>.Is.Equal(_player.Id), Arg<IDataContext>.Is.Anything)).Return(_expectedLongestWinningStreak);

            _expectedPlayerAchievementWinnersPageableList = new List<PlayerAchievementWinner>
            {
                new PlayerAchievementWinner(),
                new PlayerAchievementWinner()
            }.ToPagedList(1, 2);

            _autoMocker.Get<IRecentPlayerAchievementsUnlockedRetriever>()
                .Expect(mock => mock.GetResults(Arg<GetRecentPlayerAchievementsUnlockedQuery>.Is.Anything))
                .Return(_expectedPlayerAchievementWinnersPageableList);
        }

        [Test]
        public void It_Sets_The_Regular_Fields()
        {
            //--arrange

            //--act
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_player.Id, 0);

            //--assert
            playerDetails.GamingGroupId.ShouldBe(_player.GamingGroupId);
            playerDetails.Active.ShouldBe(_player.Active);
            playerDetails.GamingGroupName.ShouldBe(_player.GamingGroup.Name);
            playerDetails.ApplicationUserId.ShouldBe(_player.ApplicationUserId);
            playerDetails.Id.ShouldBe(_player.Id);
            playerDetails.Name.ShouldBe(_player.Name);
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfThePlayerDoesNotExist()
        {
            const int invalidPlayerId = -1;
            var expectedException = new EntityDoesNotExistException<Player>(invalidPlayerId);
            Exception actualException = Assert.Throws<EntityDoesNotExistException<Player>>(
                                                                            () => _autoMocker.ClassUnderTest.GetPlayerDetails(
                                                                                invalidPlayerId, 
                                                                                0));
            
            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItSetsTheCurrentNemesis()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_player.Id, _numberOfRecentGames);

            Assert.AreSame(_expectedNemesis, playerDetails.CurrentNemesis);
        }

        [Test]
        public void ItSetsTheCurrentNemesisToANullNemesisIfThereIsNoNemesis()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_playerWithNoNemesisEver.Id, _numberOfRecentGames);

            Assert.True(playerDetails.CurrentNemesis is NullNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesis()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_player.Id, _numberOfRecentGames);

            Assert.AreSame(_expectedPriorNemesis, playerDetails.PreviousNemesis);
        }

        [Test]
        public void ItSetsThePreviousNemesisToANullNemesisIfThereIsNoPreviousNemesis()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_playerWithOnlyACurrentNemesis.Id, _numberOfRecentGames);

            Assert.True(playerDetails.PreviousNemesis is NullNemesis);
        }

        [Test]
        public void ItSetsThePlayersMinions()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_player.Id, _numberOfRecentGames);

            Assert.AreSame(_expectedMinions, playerDetails.Minions);
        }

        [Test]
        public void ItSetsThePlayersGameSummaries()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_player.Id, _numberOfRecentGames);

            Assert.AreSame(_expectedPlayerGameSummaries, playerDetails.PlayerGameSummaries);
        }

        [Test]
        public void ItSetsThePlayersChampionedGames()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_playerWithAChampionship.Id,
                _numberOfRecentGames);

            Assert.That(playerDetails.ChampionedGames, Is.EqualTo(_expectedChampionedGames));
        }

        [Test]
        public void ItSetsThePlayersFormerChampionedGames()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_playerWithAChampionship.Id,
                _numberOfRecentGames);

            Assert.That(playerDetails.FormerChampionedGames, Is.EqualTo(_expectedFormerChampionedGames));
        }

        [Test]
        public void ItSetsThePlayerVersusPlayersStatistics()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_playerWithAChampionship.Id,
                _numberOfRecentGames);

            Assert.That(playerDetails.PlayerVersusPlayersStatistics, Is.EqualTo(_expectedPlayerVersusPlayerStatistics));
        }

        [Test]
        public void ItSetsTheLongestWinningStreak()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_player.Id, _numberOfRecentGames);

            Assert.That(playerDetails.LongestWinningStreak, Is.EqualTo(_expectedLongestWinningStreak)); 
        }

        [Test]
        public void ItSetsTheNemePointsSummary()
        {
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_player.Id, _numberOfRecentGames);

            Assert.That(playerDetails.NemePointsSummary, Is.EqualTo(_expectedPlayerStatistics.NemePointsSummary));
        }

        [Test]
        public void ItSetsThePlayersAchievements()
        {
            //--act
            var playerDetails = _autoMocker.ClassUnderTest.GetPlayerDetails(_player.Id, _numberOfRecentGames);

            //--assert
            var arguments =
                _autoMocker.Get<IRecentPlayerAchievementsUnlockedRetriever>()
                    .GetArgumentsForCallsMadeOn(x => x.GetResults(Arg<GetRecentPlayerAchievementsUnlockedQuery>.Is.Anything));
            var query = arguments.AssertFirstCallIsType<GetRecentPlayerAchievementsUnlockedQuery>();
            Assert.That(query.PlayerId, Is.EqualTo(_player.Id));
            Assert.That(query.Page, Is.EqualTo(1));
            Assert.That(query.PageSize, Is.EqualTo(int.MaxValue));

            Assert.That(playerDetails.Achievements, Is.EqualTo(_expectedPlayerAchievementWinnersPageableList));
        }
    }
}
