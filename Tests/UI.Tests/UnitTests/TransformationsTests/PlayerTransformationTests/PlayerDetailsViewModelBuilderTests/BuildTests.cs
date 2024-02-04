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
using BusinessLogic.Logic;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Points;
using Shouldly;
using StructureMap.AutoMocking;
using UI.Models.Achievements;
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
        private RhinoAutoMocker<PlayerDetailsViewModelBuilder> _autoMocker;
        private PlayerDetails _playerDetails;
        private PlayerDetailsViewModel _playerDetailsViewModel;
        private ApplicationUser _currentUser;
        private PlayerVersusPlayerStatistics _normalPlayer;
        private PlayerVersusPlayerStatistics _nemesisPlayer;
        private PlayerVersusPlayerStatistics _previousNemesisPlayer;
        private PlayerVersusPlayerStatistics _minionPlayer;
        private PlayerVersusPlayerStatistics _playerWithNoGamesPlayed;
        private readonly string _twitterMinionBraggingUrl = "some url";
        private readonly int _gamingGroupId = 123;
        private readonly int _playerId = 567;
        private readonly int _gameDefinitionIdThatIsChampionedByCurrentPlayer = 999;
        private readonly int _gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer = 1000;
        private GameDefinition _gameDefinitionThatIsFormerlyChampionedByCurrentPlayer;
        private readonly int _gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer = 1001;
        private GameDefinition _gameDefinitionThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer;
        private Dictionary<int, string> _expectedPlayerIdToRegisteredEmailDictionary;

        [SetUp]
        public void TestFixtureSetUp()
        {
            _autoMocker = new RhinoAutoMocker<PlayerDetailsViewModelBuilder>();

            //TODO shouldn't have to do this. Probably need a test base
            AutomapperConfiguration.Configure();

            _currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = _gamingGroupId,
                Id = "application user id"
            };

            var gameDefinition1 = new GameDefinition()
            {
                Name = "test game 1",
                Id = _gameDefinitionIdThatIsChampionedByCurrentPlayer,
            };
            _gameDefinitionThatIsFormerlyChampionedByCurrentPlayer = new GameDefinition
            {
                Name = "formerly championed game",
                Id = _gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer
            };
            _gameDefinitionThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer = new GameDefinition
            {
                Name = "currently and formerly championed game",
                Id = _gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer
            };
            var playedGame1 = new PlayedGame()
            {
                Id = 1,
                GameDefinition = gameDefinition1
            };

            var playedGame2 = new PlayedGame()
            {
                Id = 2,
                GameDefinitionId = _gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer
            };
            var playerGameResults = new List<PlayerGameResult>()
            {
                new PlayerGameResult(){ PlayedGameId = 12, PlayedGame = playedGame1 },
                new PlayerGameResult(){ PlayedGameId = 13, PlayedGame = playedGame2 }
            };

            _normalPlayer = new PlayerVersusPlayerStatistics
            {
                OpposingPlayerName = "Jim",
                OpposingPlayerId = 1,
                OpposingPlayerActive = false,
                NumberOfGamesWonVersusThisPlayer = 10,
                NumberOfGamesLostVersusThisPlayer = 10
            };

            _playerWithNoGamesPlayed = new PlayerVersusPlayerStatistics
            {
                OpposingPlayerId = 2
            };

            _minionPlayer = new PlayerVersusPlayerStatistics
            {
                OpposingPlayerName = "Minion Player",
                OpposingPlayerId = 5,
                NumberOfGamesWonVersusThisPlayer = 20,
                NumberOfGamesLostVersusThisPlayer = 0
            };

            _nemesisPlayer = new PlayerVersusPlayerStatistics
            {
                OpposingPlayerName = "nemesis player",
                OpposingPlayerId = 3,
                NumberOfGamesWonVersusThisPlayer = 0,
                NumberOfGamesLostVersusThisPlayer = 100
            };

            _previousNemesisPlayer = new PlayerVersusPlayerStatistics

            {
                OpposingPlayerName = "Previous Nemesis Player",
                OpposingPlayerId = 13,
                NumberOfGamesWonVersusThisPlayer = 0,
                NumberOfGamesLostVersusThisPlayer = 42
            };

            var nemesis = new Nemesis()
            {
                NemesisPlayerId = _nemesisPlayer.OpposingPlayerId,
                NumberOfGamesLost = 3,
                LossPercentage = 75,
                NemesisPlayer = new Player()
                {
                    Name = "Ace Nemesis",
                }
            };

            var previousNemesis = new Nemesis()
            {
                NemesisPlayerId = _previousNemesisPlayer.OpposingPlayerId,
                NumberOfGamesLost = 5,
                LossPercentage = 66,
                NemesisPlayer = new Player()
                {
                    Name = "Bravo Nemesis",
                }
            };

            _playerDetails = new PlayerDetails()
            {
                Id = _playerId,
                ApplicationUserId = _currentUser.Id,
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
                        Id = _minionPlayer.OpposingPlayerId
                    }
                },
                GamingGroupId = _gamingGroupId,
                GamingGroupName = "gaming group name",
                PlayerVersusPlayersStatistics = new List<PlayerVersusPlayerStatistics>
                {
                    _normalPlayer,
                    _playerWithNoGamesPlayed,
                    _nemesisPlayer,
                    _previousNemesisPlayer,
                    _minionPlayer
                },
                PlayerGameSummaries = new List<PlayerGameSummary>
                {
                    new PlayerGameSummary
                    {
                        GameDefinitionId = _gameDefinitionIdThatIsChampionedByCurrentPlayer
                    },
                    new PlayerGameSummary
                    {
                        GameDefinitionId = _gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer
                    },
                    new PlayerGameSummary
                    {
                        GameDefinitionId = _gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer
                    }
                },
                NemePointsSummary = new NemePointsSummary(1, 3, 5),
                Achievements = new List<PlayerAchievementWinner>
                {
                    new PlayerAchievementWinner()
                }
            };

            _autoMocker.Get<IGameResultViewModelBuilder>().Expect(build => build.Build(playerGameResults[0]))
                    .Repeat
                    .Once()
                    .Return(new Models.PlayedGame.GameResultViewModel() { PlayedGameId = playerGameResults[0].PlayedGameId });
            _autoMocker.Get<IGameResultViewModelBuilder>().Expect(build => build.Build(playerGameResults[1]))
                    .Repeat
                    .Once()
                    .Return(new Models.PlayedGame.GameResultViewModel() { PlayedGameId = playerGameResults[1].PlayedGameId });
            foreach (var player in _playerDetails.Minions)
            {
                _autoMocker.Get<IMinionViewModelBuilder>().Expect(mock => mock.Build(player))
                    .Return(new MinionViewModel() { MinionPlayerId = player.Id });
            }

            var championedGames = new List<Champion>
            {
                new Champion { GameDefinition = gameDefinition1, GameDefinitionId = _gameDefinitionIdThatIsChampionedByCurrentPlayer },
                new Champion { GameDefinition = _gameDefinitionThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer, GameDefinitionId = _gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer }
            };
            _playerDetails.ChampionedGames = championedGames;

            var formerChampionedGames = new List<GameDefinition>
            {
                _gameDefinitionThatIsFormerlyChampionedByCurrentPlayer,
                _gameDefinitionThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer
            };
            _playerDetails.FormerChampionedGames = formerChampionedGames;

            foreach (var achievement in _playerDetails.Achievements)
            {
                _autoMocker.Get<ITransformer>()
                    .Expect(mock => mock.Transform<PlayerAchievementSummaryViewModel>(achievement))
                    .Return(new PlayerAchievementSummaryViewModel());
            }

            _expectedPlayerIdToRegisteredEmailDictionary = new Dictionary<int, string>
            {
                {
                    _normalPlayer.OpposingPlayerId,
                    "normalplayeremail@email.com"
                }
            };

            _playerDetailsViewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser);
        }

        [Test]
        public void PlayerDetailsCannotBeNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                    _autoMocker.ClassUnderTest.Build(null, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser));

            Assert.AreEqual("playerDetails", exception.ParamName);
        }

        [Test]
        public void ItRequiresPlayerGameResults()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                    _autoMocker.ClassUnderTest.Build(new PlayerDetails(), _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresPlayerStatistics()
        {
            var playerDetailsWithNoStatistics = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            var exception = Assert.Throws<ArgumentException>(() =>
                    _autoMocker.ClassUnderTest.Build(playerDetailsWithNoStatistics, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresThePlayerIdToRegisteredUserEmailDictionary()
        {
            //--arrange

            //--act
            var exception = Assert.Throws<ArgumentException>(() =>
                _autoMocker.ClassUnderTest.Build(_playerDetails, null, _twitterMinionBraggingUrl, _currentUser));

            //--assert
            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_DICTIONARY_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void MinionsCannotBeNull()
        {
            var playerDetailsWithNoMinions = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            playerDetailsWithNoMinions.PlayerStats = new PlayerStatistics();

            var exception = Assert.Throws<ArgumentException>(() =>
                    _autoMocker.ClassUnderTest.Build(playerDetailsWithNoMinions, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_MINIONS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ChampionedGamesCannotBeNull()
        {
            var playerDetailsWithNoChampionedGames = new PlayerDetails()
            {
                PlayerGameResults = new List<PlayerGameResult>(),
                PlayerStats = new PlayerStatistics(),
                Minions = new List<Player>(),
                FormerChampionedGames = new List<GameDefinition>()
            };

            var exception = Assert.Throws<ArgumentException>(() =>
                    _autoMocker.ClassUnderTest.Build(playerDetailsWithNoChampionedGames, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_CHAMPIONED_GAMES_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void FormerChampionedGamesCannotBeNull()
        {
            var playerDetailsWithNoChampionedGames = new PlayerDetails()
            {
                PlayerGameResults = new List<PlayerGameResult>(),
                PlayerStats = new PlayerStatistics(),
                Minions = new List<Player>(),
                ChampionedGames = new List<Champion>()
            };

            var exception = Assert.Throws<ArgumentException>(() =>
                    _autoMocker.ClassUnderTest.Build(playerDetailsWithNoChampionedGames, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_FORMERCHAMPIONED_GAMES_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Assert.AreEqual(_playerDetails.Id, _playerDetailsViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            var expectedName = PlayerNameBuilder.BuildPlayerName(_playerDetails.Name, _playerDetails.Active);
            Assert.AreEqual(expectedName, _playerDetailsViewModel.PlayerName);
        }

        [Test]
        public void ItSetsThePlayerRegisteredFlagToTrueIfThereIsAnApplicationUserIdOnThePlayer()
        {
            Assert.AreEqual(true, _playerDetailsViewModel.PlayerRegistered);
        }

        [Test]
        public void ItSetsThePlayerRegisteredFlagToFalseIfThereIsNoApplicationUserIdOnThePlayer()
        {
            _playerDetails.ApplicationUserId = null;
            _playerDetailsViewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser);

            Assert.AreEqual(false, _playerDetailsViewModel.PlayerRegistered);
        }

        [Test]
        public void It_Sets_The_RegisteredUserEmailAddress_If_It_Is_In_The_Dictionary()
        {
            //--arrange
            _playerDetails.Id = _expectedPlayerIdToRegisteredEmailDictionary.Keys.First();

            //--act
            _playerDetailsViewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser);

            //--assert
            _playerDetailsViewModel.RegisteredUserEmailAddress.ShouldBe(_expectedPlayerIdToRegisteredEmailDictionary[_playerDetails.Id]);
        }

        [Test]
        public void It_Doesnt_Set_The_RegisteredUserEmailAddress_If_It_Is_Not_In_The_Dictionary()
        {
            //--arrange
            var emptyDictionary = new Dictionary<int, string>();

            //--act
            _playerDetailsViewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, emptyDictionary, _twitterMinionBraggingUrl, _currentUser);

            //--assert
            _playerDetailsViewModel.RegisteredUserEmailAddress.ShouldBeNull();
        }

        [Test]
        public void ItCopiesTheActiveFlag()
        {
            Assert.AreEqual(_playerDetails.Active, _playerDetailsViewModel.Active);
        }

        [Test]
        public void ItCopiesTheTotalGamesPlayed()
        {
            Assert.AreEqual(_playerDetails.PlayerStats.TotalGames, _playerDetailsViewModel.TotalGamesPlayed);
        }

        [Test]
        public void ItCopiesTheTotalGamesWon()
        {
            Assert.AreEqual(_playerDetails.PlayerStats.TotalGamesWon, _playerDetailsViewModel.TotalGamesWon);
        }

        [Test]
        public void ItCopiesTheTotalGamesLost()
        {
            Assert.AreEqual(_playerDetails.PlayerStats.TotalGamesLost, _playerDetailsViewModel.TotalGamesLost);
        }

        [Test]
        public void ItCopiesTheWinPercentage()
        {
            Assert.AreEqual(_playerDetails.PlayerStats.WinPercentage, _playerDetailsViewModel.WinPercentage);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(_playerDetails.GamingGroupId, _playerDetailsViewModel.GamingGroupId);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(_playerDetails.GamingGroupName, _playerDetailsViewModel.GamingGroupName);
        }

    
        [Test]
        public void ItCopiesTheNemePointsSummary()
        {
            var expected = new NemePointsSummaryViewModel(_playerDetails.PlayerStats.NemePointsSummary);
            Assert.AreEqual(expected, _playerDetailsViewModel.NemePointsSummary);
        }

        [Test]
        public void ItSetsTheAveragePointsPerGame()
        {
            var expectedPoints = (float)_playerDetails.PlayerStats.NemePointsSummary.TotalPoints / (float)_playerDetails.PlayerStats.TotalGames;

            Assert.AreEqual(expectedPoints, _playerDetailsViewModel.AveragePointsPerGame);
        }

        [Test]
        public void ItSetsTheAveragePointsPerGameToZeroIfNoGamesHaveBeenPlayed()
        {
            _playerDetails.PlayerStats.TotalGames = 0;

            _playerDetailsViewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser);

            Assert.AreEqual(0, _playerDetailsViewModel.AveragePointsPerGame);
        }

        [Test]
        public void ItSetsTheAveragePlayersPerGame()
        {
            Assert.AreEqual(_playerDetails.PlayerStats.AveragePlayersPerGame, _playerDetailsViewModel.AveragePlayersPerGame);
        }

        [Test]
        public void ItSetsTheAveragePointsPerPlayer()
        {
            var expectedPointsPerGame = (float)_playerDetails.PlayerStats.NemePointsSummary.TotalPoints / (float)_playerDetails.PlayerStats.TotalGames;
            var expectedPointsPerPlayer = expectedPointsPerGame / (float)_playerDetails.PlayerStats.AveragePlayersPerGame;

            Assert.That(expectedPointsPerPlayer, Is.EqualTo(_playerDetailsViewModel.AveragePointsPerPlayer).Within(.001));
        }

        [Test]
        public void ItSetsTheAveragePointsPerPlayerToZeroIfTheAveragePlayersPerGameIsZero()
        {
            _playerDetails.PlayerStats.AveragePlayersPerGame = 0;

            var viewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser);

            Assert.AreEqual(0, viewModel.AveragePointsPerPlayer);
        }

        [Test]
        public void ItPopulatesThePlayerGameResultDetails()
        {
            var numberOfPlayerGameResults = _playerDetails.PlayerGameResults.Count();
            int expectedPlayedGameId;
            int actualPlayedGameId;
            for (var i = 0; i < numberOfPlayerGameResults; i++)
            {
                expectedPlayedGameId = _playerDetails.PlayerGameResults[i].PlayedGameId;
                actualPlayedGameId = _playerDetailsViewModel.PlayerGameResultDetails[i].PlayedGameId;
                Assert.AreEqual(expectedPlayedGameId, actualPlayedGameId);
            }
        }

        [Test]
        public void ItPopulatesTheHasNemesisFlagIfTheNemesisIsNotNull()
        {
            Assert.IsTrue(_playerDetailsViewModel.HasNemesis);
        }

        [Test]
        public void ItPopulatesTheNemesisPlayerId()
        {
            Assert.AreEqual(_playerDetails.CurrentNemesis.NemesisPlayerId, _playerDetailsViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItPopulatesTheNemesisName()
        {
            Assert.AreEqual(_playerDetails.CurrentNemesis.NemesisPlayer.Name, _playerDetailsViewModel.NemesisName);
        }

        [Test]
        public void ItPopulatesTheGamesLostVersusTheNemesis()
        {
            Assert.AreEqual(_playerDetails.CurrentNemesis.NumberOfGamesLost, _playerDetailsViewModel.NumberOfGamesLostVersusNemesis);
        }

        [Test]
        public void ItPopulatesTheLostPercentageVersusTheNemesis()
        {
            Assert.AreEqual(_playerDetails.CurrentNemesis.LossPercentage, _playerDetailsViewModel.LossPercentageVersusPlayer);
        }

        [Test]
        public void ItSetsTheMinions()
        {
            foreach (var player in _playerDetails.Minions)
            {
                Assert.True(_playerDetailsViewModel.Minions.Any(minion => minion.MinionPlayerId == player.Id));
            }
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            var viewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser);

            Assert.True(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            _currentUser.CurrentGamingGroupId = -1;
            var viewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser);

            Assert.False(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            var viewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, null);

            Assert.False(viewModel.UserCanEdit);
        }

        [Test]
        public void ItCopiesThePlayerGameSummaries()
        {
            //--arrange
            foreach (var playerGameSummary in _playerDetails.PlayerGameSummaries)
            {
                _autoMocker.Get<ITransformer>()
                    .Expect(mock => mock.Transform<PlayerGameSummaryViewModel>(playerGameSummary))
                    .Return(new PlayerGameSummaryViewModel());

            }

            //--act
            var viewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, null);

            //--assert
            Assert.That(viewModel.PlayerGameSummaries.Count, Is.EqualTo(_playerDetails.PlayerGameSummaries.Count));
        }

        [Test]
        public void ItSetsTheChampionedGames()
        {
            Assert.That(_playerDetailsViewModel.PlayerGameSummaries.Any(
                x => x.GameDefinitionId == _gameDefinitionIdThatIsChampionedByCurrentPlayer
                    && x.IsChampion));
            Assert.That(_playerDetailsViewModel.PlayerGameSummaries.Any(
              x => x.GameDefinitionId == _gameDefinitionIdThatIsBothCurrentlyAndFormerlyChampionedByCurrentPlayer
                  && x.IsChampion));
        }

        [Test]
        public void ItSetsTheFormerChampionedGamesToIncludeOnlyGamesThatAreNotCurrentlyChampioned()
        {
            Assert.That(_playerDetailsViewModel.PlayerGameSummaries
                .Where(x => x.IsFormerChampion).All(y => y.GameDefinitionId == _gameDefinitionIdThatIsFormerlyChampionedByCurrentPlayer));
        }

        [Test]
        public void ItDoesntSetAnyCurrentlyChampionedGamesAsFormerChampionedGames()
        {
            Assert.That(!_playerDetailsViewModel.PlayerGameSummaries.Any(
                x => x.GameDefinitionId == _gameDefinitionIdThatIsChampionedByCurrentPlayer
                    && x.IsFormerChampion));
        }

        [Test]
        public void ItSetsTheTwitterBraggingUrlIfTCurrentUserIsLookingAtThemself()
        {
            Assert.That(_twitterMinionBraggingUrl, Is.EqualTo(_playerDetailsViewModel.MinionBraggingTweetUrl));
        }

        [Test]
        public void ItDoesNotSetTheTwitterBraggingUrlIfTCurrentUserIsNotThePlayerBeingTransformed()
        {
            _currentUser.Id = "some different user id";
            var viewModel = _autoMocker.ClassUnderTest.Build(_playerDetails, _expectedPlayerIdToRegisteredEmailDictionary, _twitterMinionBraggingUrl, _currentUser);

            Assert.That(null, Is.EqualTo(viewModel.MinionBraggingTweetUrl));
        }

        [Test]
        public void ItSetsTheWinLossHeader()
        {
            Assert.That(_playerDetailsViewModel.PlayerVersusPlayers.WinLossHeader, Is.EqualTo("Win - Loss Record vs. Player"));
        }

        [Test]
        public void ItPopulatesTheOpposingPlayerId()
        {
            Assert.That(_playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.Any(opposingPlayer => opposingPlayer.PlayerId == _normalPlayer.OpposingPlayerId));
        }

        [Test]
        public void ItPopulatesTheOpposingPlayerName()
        {
            var expectedPlayer = _playerDetails.PlayerVersusPlayersStatistics.First(x => x.OpposingPlayerId == _normalPlayer.OpposingPlayerId);
            var actualPlayer = _playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == _normalPlayer.OpposingPlayerId);
            var expectedPlayerName = PlayerNameBuilder.BuildPlayerName(expectedPlayer.OpposingPlayerName, expectedPlayer.OpposingPlayerActive);

            Assert.That(actualPlayer.PlayerName,
                Is.EqualTo(expectedPlayerName));
        }

        [Test]
        public void ItPopulatesTheRegisteredUserEmailAddressOfThePlayerVersusPlayerRecords()
        {
            Assert.That(_playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.Any(opposingPlayer => 
                opposingPlayer.PlayerId == _normalPlayer.OpposingPlayerId
                && opposingPlayer.RegisteredUserEmailAddress == _expectedPlayerIdToRegisteredEmailDictionary[opposingPlayer.PlayerId]));
        }

        [Test]
        public void ItPopulatesTheNumberOfGamesLostVersusThisPlayer()
        {
            var expectedPlayer = _playerDetails.PlayerVersusPlayersStatistics.First(x => x.OpposingPlayerId == _normalPlayer.OpposingPlayerId);
            var actualPlayer = _playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == _normalPlayer.OpposingPlayerId);

            Assert.That(actualPlayer.GamesLost,
                Is.EqualTo(expectedPlayer.NumberOfGamesLostVersusThisPlayer));
        }

        [Test]
        public void ItPopulatesTheNumberOfGamesWonVersusThisPlayer()
        {
            var expectedPlayer = _playerDetails.PlayerVersusPlayersStatistics.First(x => x.OpposingPlayerId == _normalPlayer.OpposingPlayerId);
            var actualPlayer = _playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == _normalPlayer.OpposingPlayerId);

            Assert.That(actualPlayer.GamesWon, Is.EqualTo(expectedPlayer.NumberOfGamesWonVersusThisPlayer));
        }

        [Test]
        public void ItPopulatesTheWinPercentageVersusThisPlayer()
        {
            var actualPlayer = _playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == _normalPlayer.OpposingPlayerId);

            Assert.That(actualPlayer.WinPercentage, Is.EqualTo(50));
        }

        [Test]
        public void TheWinPercentageIsZeroIfThereAreNoPlayedGamesVersusThisPlayer()
        {
            var actualPlayer = _playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == _playerWithNoGamesPlayed.OpposingPlayerId);

            Assert.That(actualPlayer.WinPercentage, Is.EqualTo(0));
        }

        [Test]
        public void ItAddsANemesisBadgeToTheNemesisPlayer()
        {
            var actualPlayer = _playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == _nemesisPlayer.OpposingPlayerId);

            Assert.True(actualPlayer.SpecialBadgeTypes.Any(badge => badge.GetType() == typeof(NemesisBadgeViewModel)));
        }

        [Test]
        public void ItAddsAPreviousNemesisBadgeToPreviousNemesisPlayer()
        {
            var actualPlayer = _playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == _previousNemesisPlayer.OpposingPlayerId);

            Assert.True(actualPlayer.SpecialBadgeTypes.Any(badge => badge.GetType() == typeof(PreviousNemesisBadgeViewModel)));
        }

        [Test]
        public void ItAddsAMinionBadgeToTheMinionPlayer()
        {
            var actualPlayer = _playerDetailsViewModel.PlayerVersusPlayers.PlayerSummaries.First(x => x.PlayerId == _minionPlayer.OpposingPlayerId);

            Assert.True(actualPlayer.SpecialBadgeTypes.Any(badge => badge.GetType() == typeof(MinionBadgeViewModel)));
        }

        [Test]
        public void ItCopiesTheLongestWinningStreak()
        {
            Assert.AreEqual(_playerDetails.LongestWinningStreak, _playerDetailsViewModel.LongestWinningStreak);
        }

        [Test]
        public void ItPopulatesThePlayerAchievement()
        {
            //--arrange

            //--act

            //--assert
            Assert.That(_playerDetailsViewModel.PlayerAchievements.Count, Is.EqualTo(_playerDetails.Achievements.Count));
        }

        [Test]
        public void ItOrdersAchievementsByTheAchievementLevelDescendingThenTheLastUpdatedDate()
        {
            //--arrange

            //--act
            //TODO should test this if we ever go to change the code again....

            //--assert
        }


    }
}