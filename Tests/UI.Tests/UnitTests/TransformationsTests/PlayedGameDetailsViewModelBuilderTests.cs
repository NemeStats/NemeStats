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
        private PlayedGameDetailsViewModelBuilder builder;
        private PlayedGame playedGame;
        private GamingGroup gamingGroup;
        private PlayedGameDetailsViewModel actualViewModel;
        private IGameResultViewModelBuilder detailsBuilder;
        private ApplicationUser currentUser;
        private int gamingGroupId = 1123;

        [SetUp]
        public void SetUp()
        {
            this.SetItAllUp(WinnerTypes.PlayerWin);
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

            this.gamingGroup = new GamingGroup
            {
                Id = this.gamingGroupId,
                Name = "gaming group name"
            };
            this.playedGame = new PlayedGame
            {
                Id = 11111,
                GameDefinition = new GameDefinition(),
                GamingGroup = this.gamingGroup,
                GameDefinitionId = 2222,
                PlayerGameResults = new List<PlayerGameResult>(),
                GamingGroupId = this.gamingGroupId,
                Notes = "some notes" + Environment.NewLine + "some notes on a separate line"
            };

            this.playedGame.PlayerGameResults.Add(new PlayerGameResult()
            {
                GameRank = gameRankStack.Pop(),
                GordonPoints = 3,
                Id = 1,
                PlayedGameId = this.playedGame.Id,
                PlayerId = 1
            });

            this.playedGame.PlayerGameResults.Add(new PlayerGameResult()
            {
                GameRank = gameRankStack.Pop(),
                GordonPoints = 2,
                Id = 2,
                PlayedGameId = this.playedGame.Id,
                PlayerId = 2
            });

            this.playedGame.PlayerGameResults.Add(new PlayerGameResult()
            {
                GameRank = gameRankStack.Pop(),
                GordonPoints = 1,
                Id = 3,
                PlayedGameId = this.playedGame.Id,
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

            this.detailsBuilder = MockRepository.GenerateMock<IGameResultViewModelBuilder>();
            this.builder = new PlayedGameDetailsViewModelBuilder(this.detailsBuilder);

            int totalPlayerGameResults = this.playedGame.PlayerGameResults.Count;
            for (int i = 0; i < totalPlayerGameResults; i++)
            {
                this.detailsBuilder.Expect(
                                      x => x.Build(this.playedGame.PlayerGameResults[i]))
                              .Repeat
                              .Once()
                              .Return(new GameResultViewModel()
                              {
                                  PlayerId = this.playedGame.PlayerGameResults[i].PlayerId
                              });
            }
            this.currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = this.gamingGroupId
            };

            this.actualViewModel = this.builder.Build(this.playedGame, this.currentUser);
        }

        [Test]
        public void ItRequiresAPlayedGame()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                    builder.Build(null, currentUser)
                );

            Assert.AreEqual("playedGame", exception.ParamName);
        }

        [Test]
        public void ItRequiresAGamingGroupOnThePlayedGame()
        {
            playedGame.GamingGroup = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playedGame, currentUser)
                );

            Assert.AreEqual(PlayedGameDetailsViewModelBuilder.EXCEPTION_MESSAGE_GAMING_GROUP_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresAGameDefinitionOnThePlayedGame()
        {
            playedGame.GameDefinition = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playedGame, currentUser)
                );

            Assert.AreEqual(PlayedGameDetailsViewModelBuilder.EXCEPTION_MESSAGE_GAME_DEFINITION_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresPlayerGameResultsOnThePlayedGame()
        {
            playedGame.PlayerGameResults = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playedGame, currentUser)
                );

            Assert.AreEqual(PlayedGameDetailsViewModelBuilder.EXCEPTION_MESSAGE_PLAYER_GAME_RESULTS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesTheGameId()
        {
            Assert.AreEqual(playedGame.GameDefinitionId, this.actualViewModel.GameDefinitionId);
        }

        [Test]
        public void ItCopiesThePlayedGameName()
        {
            Assert.AreEqual(playedGame.GameDefinition.Name, this.actualViewModel.GameDefinitionName);
        }

        [Test]
        public void ItCopiesThePlayedGameId()
        {
            Assert.AreEqual(playedGame.Id, this.actualViewModel.PlayedGameId);
        }

        [Test]
        public void ItCopiesTheDatePlayed()
        {
            Assert.AreEqual(playedGame.DatePlayed.Ticks, this.actualViewModel.DatePlayed.Ticks);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(playedGame.GamingGroup.Id, this.actualViewModel.GamingGroupId);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(playedGame.GamingGroup.Name, this.actualViewModel.GamingGroupName);
        }

        [Test]
        public void ItCopiesTheNotesReplacingNewlinesWithBreakTags()
        {
            Assert.That(playedGame.Notes.Replace(Environment.NewLine, PlayedGameDetailsViewModelBuilder.NEWLINE_REPLACEMENT_FOR_HTML), Is.EqualTo(this.actualViewModel.Notes));
        }

        [Test]
        public void ItSetsTheWinnerTypeToTeamWinIfAllPlayersAreTheWinner()
        {
            this.SetItAllUp(WinnerTypes.TeamWin);
            Assert.That(actualViewModel.WinnerType, Is.EqualTo(WinnerTypes.TeamWin));
        }

        [Test]
        public void ItSetsTheWinnerTypeToTeamLossIfAllPlayersAreTheWinner()
        {
            this.SetItAllUp(WinnerTypes.TeamLoss);
            Assert.That(actualViewModel.WinnerType, Is.EqualTo(WinnerTypes.TeamLoss));
        }

        [Test]
        public void ItSetsTheWinnerTypeToPlayerWinIfASubsetOfPlayersWon()
        {
            this.SetItAllUp(WinnerTypes.PlayerWin);
            Assert.That(actualViewModel.WinnerType, Is.EqualTo(WinnerTypes.PlayerWin));
        }

        [Test]
        public void ItTransformsPlayedGameResultsIntoPlayerGameResultSummaries()
        {
            for (int i = 0; i < playedGame.PlayerGameResults.Count; i++)
            {
                Assert.AreEqual(playedGame.PlayerGameResults[i].PlayerId, this.actualViewModel.PlayerResults[i].PlayerId);
            }
        }

        [Test]
        public void TheUserCanEditThePlayedGameDetailsViewModelIfTheyShareGamingGroups()
        {
            PlayedGameDetailsViewModel viewModel = builder.Build(playedGame, currentUser);

            Assert.True(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditThePlayedGameDetailsViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            PlayedGameDetailsViewModel viewModel = builder.Build(playedGame, currentUser);

            Assert.False(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditThePlayedGameDetailsViewModelIfTheCurrentUserIsUnknown()
        {
            PlayedGameDetailsViewModel viewModel = builder.Build(playedGame, null);

            Assert.False(viewModel.UserCanEdit);
        }
    }
}
