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

#endregion LICENSE

using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Points;
using UI.Mappers;
using UI.Models.Badges;
using UI.Models.Players;
using UI.Models.Points;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests.PlayerDetailsViewModelBuilderTests
{
    [TestFixture]
    public class BuildTests
    {
        private IGameResultViewModelBuilder gameResultViewModelBuilder;
        private IMinionViewModelBuilder minionViewModelBuilderMock;
        private PlayerAchievementToPlayerAchievementSummaryViewModelMapper playerAchievementViewModelBuilderMock;
        private PlayerDetails playerDetails;
        private PlayerDetailsViewModel playerDetailsViewModel;
        private PlayerDetailsViewModelBuilder builder;
        private ApplicationUser currentUser;
        private PlayerVersusPlayerStatistics normalPlayer;
        private PlayerVersusPlayerStatistics nemesisPlayer;
        private PlayerVersusPlayerStatistics previousNemesisPlayer;
        private PlayerVersusPlayerStatistics minionPlayer;
        private PlayerVersusPlayerStatistics playerWithNoGamesPlayed;
        private readonly string twitterMinionBraggingUrl = "some url";
        private readonly int gamingGroupId = 123;
        private readonly int playerId = 567;
        private readonly int gameDefinitionIdThatIsChampionedByCurrentPlayer = 999;
        private readonly int gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer = 1000;
        private GameDefinition gameDefinitionThatIsFormerlyChampionedByCurrentPlayer;
        private readonly int gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer = 1001;
        private GameDefinition gameDefinitionThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer;

        [SetUp]
        public void TestFixtureSetUp()
        {
            AutomapperConfiguration.Configure();
            minionViewModelBuilderMock = MockRepository.GenerateMock<IMinionViewModelBuilder>();
            playerAchievementViewModelBuilderMock = MockRepository.GenerateStub<PlayerAchievementToPlayerAchievementSummaryViewModelMapper>();

            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = gamingGroupId,
                Id = "application user id"
            };

            var gameDefinition1 = new GameDefinition()
            {
                Name = "test game 1",
                Id = gameDefinitionIdThatIsChampionedByCurrentPlayer,
            };
            gameDefinitionThatIsFormerlyChampionedByCurrentPlayer = new GameDefinition
            {
                Name = "formerly championed game",
                Id = gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer
            };
            gameDefinitionThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer = new GameDefinition
            {
                Name = "currently and formerly championed game",
                Id = gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer
            };
            var playedGame1 = new PlayedGame()
            {
                Id = 1,
                GameDefinition = gameDefinition1
            };

            var playedGame2 = new PlayedGame()
            {
                Id = 2,
                GameDefinitionId = gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer
            };
            var playerGameResults = new List<PlayerGameResult>()
            {
                new PlayerGameResult(){ PlayedGameId = 12, PlayedGame = playedGame1 },
                new PlayerGameResult(){ PlayedGameId = 13, PlayedGame = playedGame2 }
            };

            normalPlayer = new PlayerVersusPlayerStatistics
            {
                OpposingPlayerName = "Jim",
                OpposingPlayerId = 1,
                OpposingPlayerActive = false,
                NumberOfGamesWonVersusThisPlayer = 10,
                NumberOfGamesLostVersusThisPlayer = 10
            };

            playerWithNoGamesPlayed = new PlayerVersusPlayerStatistics
            {
                OpposingPlayerId = 2
            };

            minionPlayer = new PlayerVersusPlayerStatistics
            {
                OpposingPlayerName = "Minion Player",
                OpposingPlayerId = 5,
                NumberOfGamesWonVersusThisPlayer = 20,
                NumberOfGamesLostVersusThisPlayer = 0
            };

            nemesisPlayer = new PlayerVersusPlayerStatistics
            {
                OpposingPlayerName = "nemesis player",
                OpposingPlayerId = 3,
                NumberOfGamesWonVersusThisPlayer = 0,
                NumberOfGamesLostVersusThisPlayer = 100
            };

            previousNemesisPlayer = new PlayerVersusPlayerStatistics

            {
                OpposingPlayerName = "Previous Nemesis Player",
                OpposingPlayerId = 13,
                NumberOfGamesWonVersusThisPlayer = 0,
                NumberOfGamesLostVersusThisPlayer = 42
            };

            var nemesis = new Nemesis()
            {
                NemesisPlayerId = nemesisPlayer.OpposingPlayerId,
                NumberOfGamesLost = 3,
                LossPercentage = 75,
                NemesisPlayer = new Player()
                {
                    Name = "Ace Nemesis",
                }
            };

            var previousNemesis = new Nemesis()
            {
                NemesisPlayerId = previousNemesisPlayer.OpposingPlayerId,
                NumberOfGamesLost = 5,
                LossPercentage = 66,
                NemesisPlayer = new Player()
                {
                    Name = "Bravo Nemesis",
                }
            };

            playerDetails = new PlayerDetails()
            {
                Id = playerId,
                ApplicationUserId = currentUser.Id,
                Active = true,
                Name = "Skipper",
                LongestWinningStreak = 39,
                PlayerGameResults = playerGameResults,
                PlayerStats = new PlayerStatistics()
                {
                    TotalGames = 5,
                    NemePointsSummary = new NemePointsSummary(1, 3, 5),
                    AveragePlayersPerGame = 3,
                    TotalGamesLost = 1,
                    TotalGamesWon = 4,
                    WinPercentage = 20
                },
                CurrentNemesis = nemesis,
                PreviousNemesis = previousNemesis,
                Minions = new List<Player>
                {
                    new Player
                    {
                        Id = minionPlayer.OpposingPlayerId
                    }
                },
                GamingGroupId = gamingGroupId,
                GamingGroupName = "gaming group name",
                PlayerVersusPlayersStatistics = new List<PlayerVersusPlayerStatistics>
                {
                    normalPlayer,
                    playerWithNoGamesPlayed,
                    nemesisPlayer,
                    previousNemesisPlayer,
                    minionPlayer
                },
                PlayerGameSummaries = new List<PlayerGameSummary>
                {
                    new PlayerGameSummary
                    {
                        GameDefinitionId = gameDefinitionIdThatIsChampionedByCurrentPlayer
                    },
                    new PlayerGameSummary
                    {
                        GameDefinitionId = gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer
                    },
                    new PlayerGameSummary
                    {
                        GameDefinitionId = gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer
                    }
                },
                NemePointsSummary = new NemePointsSummary(1, 3, 5)
            };

            gameResultViewModelBuilder
                = MockRepository.GenerateMock<IGameResultViewModelBuilder>();
            gameResultViewModelBuilder.Expect(build => build.Build(playerGameResults[0]))
                    .Repeat
                    .Once()
                    .Return(new Models.PlayedGame.GameResultViewModel() { PlayedGameId = playerGameResults[0].PlayedGameId });
            gameResultViewModelBuilder.Expect(build => build.Build(playerGameResults[1]))
                    .Repeat
                    .Once()
                    .Return(new Models.PlayedGame.GameResultViewModel() { PlayedGameId = playerGameResults[1].PlayedGameId });
            foreach (var player in playerDetails.Minions)
            {
                minionViewModelBuilderMock.Expect(mock => mock.Build(player))
                    .Return(new MinionViewModel() { MinionPlayerId = player.Id });
            }

            var championedGames = new List<Champion>
            {
                new Champion { GameDefinition = gameDefinition1, GameDefinitionId = gameDefinitionIdThatIsChampionedByCurrentPlayer },
                new Champion { GameDefinition = gameDefinitionThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer, GameDefinitionId = gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer }
            };
            playerDetails.ChampionedGames = championedGames;

            var formerChampionedGames = new List<GameDefinition>
            {
                gameDefinitionThatIsFormerlyChampionedByCurrentPlayer,
                gameDefinitionThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer
            };
            playerDetails.FormerChampionedGames = formerChampionedGames;

          

            builder = new PlayerDetailsViewModelBuilder(gameResultViewModelBuilder, minionViewModelBuilderMock, playerAchievementViewModelBuilderMock);

            playerDetailsViewModel = builder.Build(playerDetails, twitterMinionBraggingUrl, currentUser);
        }

        [Test]
        public void PlayerDetailsCannotBeNull()
        {
            var builder = new PlayerDetailsViewModelBuilder(null, null, null);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                    builder.Build(null, twitterMinionBraggingUrl, currentUser));

            Assert.AreEqual("playerDetails", exception.ParamName);
        }

        [Test]
        public void ItRequiresPlayerGameResults()
        {
            var builder = new PlayerDetailsViewModelBuilder(null, null, null);

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(new PlayerDetails(), twitterMinionBraggingUrl, currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresPlayerStatistics()
        {
            var builder = new PlayerDetailsViewModelBuilder(null, null, null);
            var playerDetailsWithNoStatistics = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerDetailsWithNoStatistics, twitterMinionBraggingUrl, currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void MinionsCannotBeNull()
        {
            var builder = new PlayerDetailsViewModelBuilder(null, null, null);
            var playerDetailsWithNoMinions = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            playerDetailsWithNoMinions.PlayerStats = new PlayerStatistics();

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerDetailsWithNoMinions, twitterMinionBraggingUrl, currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_MINIONS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ChampionedGamesCannotBeNull()
        {
            var builder = new PlayerDetailsViewModelBuilder(null, null,null);
            var playerDetailsWithNoChampionedGames = new PlayerDetails()
            {
                PlayerGameResults = new List<PlayerGameResult>(),
                PlayerStats = new PlayerStatistics(),
                Minions = new List<Player>(),
                FormerChampionedGames = new List<GameDefinition>()
            };

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerDetailsWithNoChampionedGames, twitterMinionBraggingUrl, currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_CHAMPIONED_GAMES_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void FormerChampionedGamesCannotBeNull()
        {
            var builder = new PlayerDetailsViewModelBuilder(null, null, null);
            var playerDetailsWithNoChampionedGames = new PlayerDetails()
            {
                PlayerGameResults = new List<PlayerGameResult>(),
                PlayerStats = new PlayerStatistics(),
                Minions = new List<Player>(),
                ChampionedGames = new List<Champion>()
            };

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerDetailsWithNoChampionedGames, twitterMinionBraggingUrl, currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_FORMERCHAMPIONED_GAMES_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Assert.AreEqual(playerDetails.Id, playerDetailsViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            var expectedName = PlayerNameBuilder.BuildPlayerName(playerDetails.Name, playerDetails.Active);
            Assert.AreEqual(expectedName, playerDetailsViewModel.PlayerName);
        }

        [Test]
        public void ItSetsThePlayerRegisteredFlagToTrueIfThereIsAnApplicationUserIdOnThePlayer()
        {
            Assert.AreEqual(true, playerDetailsViewModel.PlayerRegistered);
        }

        [Test]
        public void ItSetsThePlayerRegisteredFlagToFalseIfThereIsNoApplicationUserIdOnThePlayer()
        {
            playerDetails.ApplicationUserId = null;
            playerDetailsViewModel = builder.Build(playerDetails, twitterMinionBraggingUrl, currentUser);

            Assert.AreEqual(false, playerDetailsViewModel.PlayerRegistered);
        }

        [Test]
        public void ItCopiesTheActiveFlag()
        {
            Assert.AreEqual(playerDetails.Active, playerDetailsViewModel.Active);
        }

        [Test]
        public void ItCopiesTheTotalGamesPlayed()
        {
            Assert.AreEqual(playerDetails.PlayerStats.TotalGames, playerDetailsViewModel.TotalGamesPlayed);
        }

        [Test]
        public void ItCopiesTheTotalGamesWon()
        {
            Assert.AreEqual(playerDetails.PlayerStats.TotalGamesWon, playerDetailsViewModel.TotalGamesWon);
        }

        [Test]
        public void ItCopiesTheTotalGamesLost()
        {
            Assert.AreEqual(playerDetails.PlayerStats.TotalGamesLost, playerDetailsViewModel.TotalGamesLost);
        }

        [Test]
        public void ItCopiesTheWinPercentage()
        {
            Assert.AreEqual(playerDetails.PlayerStats.WinPercentage, playerDetailsViewModel.WinPercentage);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(playerDetails.GamingGroupId, playerDetailsViewModel.GamingGroupId);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(playerDetails.GamingGroupName, playerDetailsViewModel.GamingGroupName);
        }

    
        [Test]
        public void ItCopiesTheNemePointsSummary()
        {
            var expected = new NemePointsSummaryViewModel(playerDetails.PlayerStats.NemePointsSummary);
            Assert.AreEqual(expected, playerDetailsViewModel.NemePointsSummary);
        }

        [Test]
        public void ItSetsTheAveragePointsPerGame()
        {
            var expectedPoints = (float)playerDetails.PlayerStats.NemePointsSummary.TotalPoints / (float)playerDetails.PlayerStats.TotalGames;

            Assert.AreEqual(expectedPoints, playerDetailsViewModel.AveragePointsPerGame);
        }

        [Test]
        public void ItSetsTheAveragePointsPerGameToZeroIfNoGamesHaveBeenPlayed()
        {
            playerDetails.PlayerStats.TotalGames = 0;

            playerDetailsViewModel = builder.Build(playerDetails, twitterMinionBraggingUrl, currentUser);

            Assert.AreEqual(0, playerDetailsViewModel.AveragePointsPerGame);
        }

        [Test]
        public void ItSetsTheAveragePlayersPerGame()
        {
            Assert.AreEqual(playerDetails.PlayerStats.AveragePlayersPerGame, playerDetailsViewModel.AveragePlayersPerGame);
        }

        [Test]
        public void ItSetsTheAveragePointsPerPlayer()
        {
            var expectedPointsPerGame = (float)playerDetails.PlayerStats.NemePointsSummary.TotalPoints / (float)playerDetails.PlayerStats.TotalGames;
            var expectedPointsPerPlayer = expectedPointsPerGame / (float)playerDetails.PlayerStats.AveragePlayersPerGame;

            Assert.AreEqual(expectedPointsPerPlayer, playerDetailsViewModel.AveragePointsPerPlayer);
        }

        [Test]
        public void ItSetsTheAveragePointsPerPlayerToZeroIfTheAveragePlayersPerGameIsZero()
        {
            playerDetails.PlayerStats.AveragePlayersPerGame = 0;

            var viewModel = builder.Build(playerDetails, twitterMinionBraggingUrl, currentUser);

            Assert.AreEqual(0, viewModel.AveragePointsPerPlayer);
        }

        [Test]
        public void ItPopulatesThePlayerGameResultDetails()
        {
            var numberOfPlayerGameResults = playerDetails.PlayerGameResults.Count();
            int expectedPlayedGameId;
            int actualPlayedGameId;
            for (var i = 0; i < numberOfPlayerGameResults; i++)
            {
                expectedPlayedGameId = playerDetails.PlayerGameResults[i].PlayedGameId;
                actualPlayedGameId = playerDetailsViewModel.PlayerGameResultDetails[i].PlayedGameId;
                Assert.AreEqual(expectedPlayedGameId, actualPlayedGameId);
            }
        }

        [Test]
        public void ItPopulatesTheHasNemesisFlagIfTheNemesisIsNotNull()
        {
            Assert.IsTrue(playerDetailsViewModel.HasNemesis);
        }

        [Test]
        public void ItPopulatesTheNemesisPlayerId()
        {
            Assert.AreEqual(playerDetails.CurrentNemesis.NemesisPlayerId, playerDetailsViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItPopulatesTheNemesisName()
        {
            Assert.AreEqual(playerDetails.CurrentNemesis.NemesisPlayer.Name, playerDetailsViewModel.NemesisName);
        }

        [Test]
        public void ItPopulatesTheGamesLostVersusTheNemesis()
        {
            Assert.AreEqual(playerDetails.CurrentNemesis.NumberOfGamesLost, playerDetailsViewModel.NumberOfGamesLostVersusNemesis);
        }

        [Test]
        public void ItPopulatesTheLostPercentageVersusTheNemesis()
        {
            Assert.AreEqual(playerDetails.CurrentNemesis.LossPercentage, playerDetailsViewModel.LossPercentageVersusPlayer);
        }

        [Test]
        public void ItSetsTheMinions()
        {
            foreach (var player in playerDetails.Minions)
            {
                Assert.True(playerDetailsViewModel.Minions.Any(minion => minion.MinionPlayerId == player.Id));
            }
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            var viewModel = builder.Build(playerDetails, twitterMinionBraggingUrl, currentUser);

            Assert.True(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            var viewModel = builder.Build(playerDetails, twitterMinionBraggingUrl, currentUser);

            Assert.False(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            var viewModel = builder.Build(playerDetails, twitterMinionBraggingUrl, null);

            Assert.False(viewModel.UserCanEdit);
        }

        [Test]
        public void ItCopiesThePlayerGameSummaries()
        {
            //todo should put an interface over top of automapper and unit test this
        }

        [Test]
        public void ItSetsTheChampionedGames()
        {
            Assert.That(playerDetailsViewModel.PlayerGameSummaries.Any(
                x => x.GameDefinitionId == gameDefinitionIdThatIsChampionedByCurrentPlayer
                    && x.IsChampion));
            Assert.That(playerDetailsViewModel.PlayerGameSummaries.Any(
              x => x.GameDefinitionId == gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer
                  && x.IsChampion));
        }

        [Test]
        public void ItSetsTheFormerChampionedGamesToIncludeOnlyGamesThatAreNotCurrentlyChampioned()
        {
            Assert.That(playerDetailsViewModel.PlayerGameSummaries
                .Where(x => x.IsFormerChampion).All(y => y.GameDefinitionId == gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer));
        }

        [Test]
        public void ItDoesntSetAnyCurrentlyChampionedGamesAsFormerChampionedGames()
        {
            Assert.That(!playerDetailsViewModel.PlayerGameSummaries.Any(
                x => x.GameDefinitionId == gameDefinitionIdThatIsChampionedByCurrentPlayer
                    && x.IsFormerChampion));
        }

        [Test]
        public void ItSetsTheTwitterBraggingUrlIfTCurrentUserIsLookingAtThemself()
        {
            Assert.That(twitterMinionBraggingUrl, Is.EqualTo(playerDetailsViewModel.MinionBraggingTweetUrl));
        }

        [Test]
        public void ItDoesNotSetTheTwitterBraggingUrlIfTCurrentUserIsNotThePlayerBeingTransformed()
        {
            currentUser.Id = "some different user id";
            var viewModel = builder.Build(playerDetails, twitterMinionBraggingUrl, currentUser);

            Assert.That(null, Is.EqualTo(viewModel.MinionBraggingTweetUrl));
        }

        [Test]
        public void ItSetsTheWinLossHeader()
        {
            Assert.That(playerDetailsViewModel.PlayerVersusPlayers.WinLossHeader, Is.EqualTo("Win - Loss Record vs. Player"));
        }

        [Test]
        public void ItPopulatesTheOpposingPlayerId()
        {
            Assert.That(playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.Any(opposingPlayer => opposingPlayer.PlayerId == normalPlayer.OpposingPlayerId));
        }

        [Test]
        public void ItPopulatesTheOpposingPlayerName()
        {
            var expectedPlayer = playerDetails.PlayerVersusPlayersStatistics.First(x => x.OpposingPlayerId == normalPlayer.OpposingPlayerId);
            var actualPlayer = playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == normalPlayer.OpposingPlayerId);
            var expectedPlayerName = PlayerNameBuilder.BuildPlayerName(expectedPlayer.OpposingPlayerName, expectedPlayer.OpposingPlayerActive);

            Assert.That(actualPlayer.PlayerName,
                Is.EqualTo(expectedPlayerName));
        }

        [Test]
        public void ItPopulatesTheNumberOfGamesLostVersusThisPlayer()
        {
            var expectedPlayer = playerDetails.PlayerVersusPlayersStatistics.First(x => x.OpposingPlayerId == normalPlayer.OpposingPlayerId);
            var actualPlayer = playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == normalPlayer.OpposingPlayerId);

            Assert.That(actualPlayer.GamesLost,
                Is.EqualTo(expectedPlayer.NumberOfGamesLostVersusThisPlayer));
        }

        [Test]
        public void ItPopulatesTheNumberOfGamesWonVersusThisPlayer()
        {
            var expectedPlayer = playerDetails.PlayerVersusPlayersStatistics.First(x => x.OpposingPlayerId == normalPlayer.OpposingPlayerId);
            var actualPlayer = playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == normalPlayer.OpposingPlayerId);

            Assert.That(actualPlayer.GamesWon, Is.EqualTo(expectedPlayer.NumberOfGamesWonVersusThisPlayer));
        }

        [Test]
        public void ItPopulatesTheWinPercentageVersusThisPlayer()
        {
            var actualPlayer = playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == normalPlayer.OpposingPlayerId);

            Assert.That(actualPlayer.WinPercentage, Is.EqualTo(50));
        }

        [Test]
        public void TheWinPercentageIsZeroIfThereAreNoPlayedGamesVersusThisPlayer()
        {
            var actualPlayer = playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == playerWithNoGamesPlayed.OpposingPlayerId);

            Assert.That(actualPlayer.WinPercentage, Is.EqualTo(0));
        }

        [Test]
        public void ItAddsANemesisBadgeToTheNemesisPlayer()
        {
            var actualPlayer = playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == nemesisPlayer.OpposingPlayerId);

            Assert.True(actualPlayer.SpecialBadgeTypes.Any(badge => badge.GetType() == typeof(NemesisBadgeViewModel)));
        }

        [Test]
        public void ItAddsAPreviousNemesisBadgeToPreviousNemesisPlayer()
        {
            var actualPlayer = playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == previousNemesisPlayer.OpposingPlayerId);

            Assert.True(actualPlayer.SpecialBadgeTypes.Any(badge => badge.GetType() == typeof(PreviousNemesisBadgeViewModel)));
        }

        [Test]
        public void ItAddsAMinionBadgeToTheMinionPlayer()
        {
            var actualPlayer = playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == minionPlayer.OpposingPlayerId);

            Assert.True(actualPlayer.SpecialBadgeTypes.Any(badge => badge.GetType() == typeof(MinionBadgeViewModel)));
        }

        [Test]
        public void ItCopiesTheLongestWinningStreak()
        {
            Assert.AreEqual(playerDetails.LongestWinningStreak, playerDetailsViewModel.LongestWinningStreak);
        }
    }
}