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
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class PlayedGameDetailsBuilderTests
    {
        private PlayedGameDetailsViewModelBuilder _builder;
        private PlayedGame _playedGame;
        private GamingGroup _gamingGroup;
        private PlayedGameDetailsViewModel _actualViewModel;
        private IGameResultViewModelBuilder _detailsBuilder;
        private ApplicationUser _currentUser;
        private int _gamingGroupId = 1123;

        [SetUp]
        public void SetUp()
        {
            SetItAllUp(WinnerTypes.PlayerWin);
        }

        private void SetItAllUp(WinnerTypes winnerType)
        {
            Stack<int> gameRankStack = new Stack<int>();
            if (winnerType == WinnerTypes.PlayerWin)
            {
                gameRankStack.Push(1);
                gameRankStack.Push(2);
                gameRankStack.Push(3);
            }else if (winnerType == WinnerTypes.TeamLoss)
            {
                gameRankStack.Push(2);
                gameRankStack.Push(2);
                gameRankStack.Push(2);
            }else if (winnerType == WinnerTypes.TeamWin)
            {
                gameRankStack.Push(1);
                gameRankStack.Push(1);
                gameRankStack.Push(1);
            }

            _gamingGroup = new GamingGroup
            {
                Id = _gamingGroupId,
                Name = "gaming group name"
            };
            _playedGame = new PlayedGame
            {
                Id = 11111,
                GameDefinition = new GameDefinition(),
                GamingGroup = _gamingGroup,
                GameDefinitionId = 2222,
                PlayerGameResults = new List<PlayerGameResult>(),
                GamingGroupId = _gamingGroupId,
                Notes = "some notes" + Environment.NewLine + "some notes on a separate line",
                WinnerType = winnerType
            };

            _playedGame.PlayerGameResults.Add(new PlayerGameResult()
            {
                GameRank = gameRankStack.Pop(),
                NemeStatsPointsAwarded = 3,
                Id = 1,
                PlayedGameId = _playedGame.Id,
                PlayerId = 1
            });

            _playedGame.PlayerGameResults.Add(new PlayerGameResult()
            {
                GameRank = gameRankStack.Pop(),
                NemeStatsPointsAwarded = 2,
                Id = 2,
                PlayedGameId = _playedGame.Id,
                PlayerId = 2
            });

            _playedGame.PlayerGameResults.Add(new PlayerGameResult()
            {
                GameRank = gameRankStack.Pop(),
                NemeStatsPointsAwarded = 1,
                Id = 3,
                PlayedGameId = _playedGame.Id,
                PlayerId = 3,
                PlayedGame = new PlayedGame()
                {
                    GameDefinition = new GameDefinition()
                    {
                        Id = 135,
                        Name = "Test game name"
                    }
                }
            });

            _detailsBuilder = MockRepository.GenerateMock<IGameResultViewModelBuilder>();
            _builder = new PlayedGameDetailsViewModelBuilder(_detailsBuilder);

            int totalPlayerGameResults = _playedGame.PlayerGameResults.Count;
            for (int i = 0; i < totalPlayerGameResults; i++)
            {
                _detailsBuilder.Expect(
                                      x => x.Build(_playedGame.PlayerGameResults[i]))
                              .Repeat
                              .Once()
                              .Return(new GameResultViewModel()
                              {
                                  PlayerId = _playedGame.PlayerGameResults[i].PlayerId
                              });
            }
            _currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = _gamingGroupId
            };

            _actualViewModel = _builder.Build(_playedGame, _currentUser);
        }

        [Test]
        public void ItRequiresAPlayedGame()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                    _builder.Build(null, _currentUser)
                );

            Assert.AreEqual("playedGame", exception.ParamName);
        }

        [Test]
        public void ItRequiresAGamingGroupOnThePlayedGame()
        {
            _playedGame.GamingGroup = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                    _builder.Build(_playedGame, _currentUser)
                );

            Assert.AreEqual(PlayedGameDetailsViewModelBuilder.EXCEPTION_MESSAGE_GAMING_GROUP_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresAGameDefinitionOnThePlayedGame()
        {
            _playedGame.GameDefinition = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                    _builder.Build(_playedGame, _currentUser)
                );

            Assert.AreEqual(PlayedGameDetailsViewModelBuilder.EXCEPTION_MESSAGE_GAME_DEFINITION_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresPlayerGameResultsOnThePlayedGame()
        {
            _playedGame.PlayerGameResults = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                    _builder.Build(_playedGame, _currentUser)
                );

            Assert.AreEqual(PlayedGameDetailsViewModelBuilder.EXCEPTION_MESSAGE_PLAYER_GAME_RESULTS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesTheGameId()
        {
            Assert.AreEqual(_playedGame.GameDefinitionId, _actualViewModel.GameDefinitionId);
        }

        [Test]
        public void ItCopiesThePlayedGameName()
        {
            Assert.AreEqual(_playedGame.GameDefinition.Name, _actualViewModel.GameDefinitionName);
        }

        [Test]
        public void ItCopiesThePlayedGameId()
        {
            Assert.AreEqual(_playedGame.Id, _actualViewModel.PlayedGameId);
        }

        [Test]
        public void ItCopiesTheDatePlayed()
        {
            Assert.AreEqual(_playedGame.DatePlayed.Ticks, _actualViewModel.DatePlayed.Ticks);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(_playedGame.GamingGroup.Id, _actualViewModel.GamingGroupId);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(_playedGame.GamingGroup.Name, _actualViewModel.GamingGroupName);
        }

        [Test]
        public void ItCopiesTheWinnerType()
        {
            SetItAllUp(WinnerTypes.TeamLoss);

            Assert.AreEqual(_playedGame.WinnerType, _actualViewModel.WinnerType);
        }

        [Test]
        public void ItCopiesTheNotesReplacingNewlinesWithBreakTags()
        {
            Assert.That(_playedGame.Notes.Replace(Environment.NewLine, PlayedGameDetailsViewModelBuilder.NEWLINE_REPLACEMENT_FOR_HTML), Is.EqualTo(_actualViewModel.Notes));
        }

        [Test]
        public void ItTransformsPlayedGameResultsIntoPlayerGameResultSummaries()
        {
            for (int i = 0; i < _playedGame.PlayerGameResults.Count; i++)
            {
                Assert.AreEqual(_playedGame.PlayerGameResults[i].PlayerId, _actualViewModel.PlayerResults[i].PlayerId);
            }
        }

        [Test]
        public void TheUserCanEditThePlayedGameDetailsViewModelIfTheyShareGamingGroups()
        {
            PlayedGameDetailsViewModel viewModel = _builder.Build(_playedGame, _currentUser);

            Assert.True(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditThePlayedGameDetailsViewModelIfTheyDoNotShareGamingGroups()
        {
            _currentUser.CurrentGamingGroupId = -1;
            PlayedGameDetailsViewModel viewModel = _builder.Build(_playedGame, _currentUser);

            Assert.False(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditThePlayedGameDetailsViewModelIfTheCurrentUserIsUnknown()
        {
            PlayedGameDetailsViewModel viewModel = _builder.Build(_playedGame, null);

            Assert.False(viewModel.UserCanEdit);
        }
    }
}
