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
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetAllPlayersWithNemesisInfoTests : PlayerRetrieverTestBase
    {
        [SetUp]
        public override void BaseSetUp()
        {
            base.BaseSetUp();

            autoMocker.ClassUnderTest.Expect(mock => mock.PopulateNemePointsSummary(Arg<int>.Is.Anything, Arg<List<PlayerWithNemesis>>.Is.Anything, Arg<IDateRangeFilter>.Is.Anything));
        }

        [Test]
        public void ItOnlyReturnsPlayersForTheGivenGamingGroup()
        {
            List<PlayerWithNemesis> players = autoMocker.ClassUnderTest.GetAllPlayersWithNemesisInfo(gamingGroupId);

            Assert.True(players.All(player => player.GamingGroupId == gamingGroupId));
        }

        [Test]
        public void ItFiltersPlayedGamesThatHappenedBeforeTheFromDate()
        {
            var autoMocker = new RhinoAutoMocker<PlayerRetriever>();
            autoMocker.PartialMockTheClassUnderTest();
            int gamingGroupId = 1;
            var fromDate = new DateTime(2015, 6, 1);
            int expectedNemePointsAwardedForEachGame = 50;
            var queryable = new List<Player>
            {
                new Player
                {
                    GamingGroupId = gamingGroupId,
                    PlayerGameResults = new List<PlayerGameResult>
                    {
                        new PlayerGameResult
                        {
                            GameRank = 2,
                            PlayedGame = new PlayedGame
                            {
                                DatePlayed = fromDate.AddDays(-1)
                            },
                            NemeStatsPointsAwarded = expectedNemePointsAwardedForEachGame
                        },
                        new PlayerGameResult
                        {
                            GameRank = 1,
                            PlayedGame = new PlayedGame
                            {
                                DatePlayed = fromDate.AddDays(1)
                            },
                            NemeStatsPointsAwarded = expectedNemePointsAwardedForEachGame
                        },
                        new PlayerGameResult
                        {
                            GameRank = 1,
                            PlayedGame = new PlayedGame
                            {
                                DatePlayed = fromDate.AddDays(-1)
                            },
                            NemeStatsPointsAwarded = expectedNemePointsAwardedForEachGame
                        }
                    },
                    ChampionedGames = new List<Champion>()
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                .Return(queryable);
            autoMocker.ClassUnderTest.Expect(mock => mock.PopulateNemePointsSummary(Arg<int>.Is.Anything, Arg<List<PlayerWithNemesis>>.Is.Anything, Arg<IDateRangeFilter>.Is.Anything));

            var dateRangeFilter = new BasicDateRangeFilter
            {
                FromDate = fromDate
            };

            var players = autoMocker.ClassUnderTest.GetAllPlayersWithNemesisInfo(gamingGroupId, dateRangeFilter);

            Assert.That(players.Count, Is.EqualTo(1));
            var player = players.First();
            Assert.That(player.GamesLost, Is.EqualTo(0));
            Assert.That(player.GamesWon, Is.EqualTo(1));
        }

        [Test]
        public void ItFiltersPlayedGamesThatHappenedAfterTheToDate()
        {
            var autoMocker = new RhinoAutoMocker<PlayerRetriever>();
            autoMocker.PartialMockTheClassUnderTest();

            int gamingGroupId = 1;
            var toDate = new DateTime(2015, 1, 1);
            int expectedNemePointsAwardedForEachGame = 50;
            var queryable = new List<Player>
            {
                new Player
                {
                    GamingGroupId = gamingGroupId,
                    PlayerGameResults = new List<PlayerGameResult>
                    {
                        new PlayerGameResult
                        {
                            GameRank = 2,
                            PlayedGame = new PlayedGame
                            {
                                DatePlayed = toDate.AddDays(-1)
                            },
                            NemeStatsPointsAwarded = expectedNemePointsAwardedForEachGame
                        },
                        new PlayerGameResult
                        {
                            GameRank = 1,
                            PlayedGame = new PlayedGame
                            {
                                DatePlayed = toDate.AddDays(1)
                            },
                            NemeStatsPointsAwarded = expectedNemePointsAwardedForEachGame
                        }
                    },
                    ChampionedGames = new List<Champion>()
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                .Return(queryable);

            autoMocker.ClassUnderTest.Expect(mock => mock.PopulateNemePointsSummary(Arg<int>.Is.Anything, Arg<List<PlayerWithNemesis>>.Is.Anything, Arg<IDateRangeFilter>.Is.Anything));


            var dateRangeFilter = new BasicDateRangeFilter
            {
                ToDate = toDate
            };

            var players = autoMocker.ClassUnderTest.GetAllPlayersWithNemesisInfo(gamingGroupId, dateRangeFilter);

            Assert.That(players.Count, Is.EqualTo(1));
            var player = players.First();
            Assert.That(player.GamesLost, Is.EqualTo(1));
            Assert.That(player.GamesWon, Is.EqualTo(0));
        }

        [Test]
        public void ItReturnsPlayersOrderedByActiveAscThenTotalPointsDescThenNameAscending()
        {
            List<PlayerWithNemesis> players = autoMocker.ClassUnderTest.GetAllPlayersWithNemesisInfo(gamingGroupId);

            var lastPlayerPoints = int.MaxValue;
            var lastPlayerName = "0";
            var lastActive = true;
            foreach (PlayerWithNemesis player in players)
            {
                if (lastActive == player.PlayerActive)
                {
                    if (lastPlayerPoints.Equals(player.NemePointsSummary?.TotalPoints ?? 0))
                    {
                        Assert.LessOrEqual(lastPlayerName, player.PlayerName);
                    }
                    else
                    {
                        Assert.GreaterOrEqual(lastPlayerPoints, player.NemePointsSummary?.TotalPoints ?? 0);
                    }
                }
                else
                {
                    //if the playerActive isn't the same as last active then it should be inactive since these come last
                    Assert.False(player.PlayerActive);
                }
               

                lastPlayerPoints = player.NemePointsSummary?.TotalPoints ?? 0;
                lastPlayerName = player.PlayerName;
                lastActive = player.PlayerActive;
            }
        }

        [Test]
        public void ItReturnsTheNumberOfGamesWon()
        {
            int expectedNumberOfGamesWon = playerGameResultsForFirstPlayer.Count(x => x.GameRank == 1);

            List<PlayerWithNemesis> players = autoMocker.ClassUnderTest.GetAllPlayersWithNemesisInfo(gamingGroupId);

            Assert.That(players[0].GamesWon, Is.EqualTo(expectedNumberOfGamesWon));
        }

        [Test]
        public void ItReturnsTheNumberOfGamesLost()
        {
            int expectedNumberOfGamesLost = playerGameResultsForFirstPlayer.Count(x => x.GameRank > 1);

            List<PlayerWithNemesis> players = autoMocker.ClassUnderTest.GetAllPlayersWithNemesisInfo(gamingGroupId);

            Assert.That(players[0].GamesLost, Is.EqualTo(expectedNumberOfGamesLost));
        }

        [Test]
        public void ItReturnsChampionships()
        {
            List<PlayerWithNemesis> players = autoMocker.ClassUnderTest.GetAllPlayersWithNemesisInfo(gamingGroupId);

            Assert.That(players[0].TotalChampionedGames, Is.EqualTo(playerChampionshipsForFirstPlayer.Count()));
        }

        [TestFixture]
        public class WhenCallingPopulateNemePointsSummary
        {
            //TODO write tests for this
            [Test]
            public void TestName()
            {
                //--arrange

                //--act
                

                //--assert
            }

        }
    }
}
