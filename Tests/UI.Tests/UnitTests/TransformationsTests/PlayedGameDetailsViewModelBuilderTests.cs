using BusinessLogic.Models;
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
        private PlayedGameDetailsViewModel playedGameDetails;
        private IGameResultViewModelBuilder detailsBuilder;
        private ApplicationUser currentUser;
        private int gamingGroupId = 1123;

        [SetUp]
        public void SetUp()
        {
            gamingGroup = new GamingGroup
            {
                Id = gamingGroupId,
                Name = "gaming group name"
            };
            playedGame = new PlayedGame()
            {
                Id = 11111,
                GameDefinition = new GameDefinition(),
                GamingGroup = gamingGroup,
                GameDefinitionId = 2222,
                PlayerGameResults = new List<PlayerGameResult>(),
                GamingGroupId = gamingGroupId,
                Notes = "some notes" + Environment.NewLine + "some notes on a separate line"
            };

            playedGame.PlayerGameResults.Add(new PlayerGameResult()
                {
                    GameRank = 1,
                    GordonPoints = 3,
                    Id = 1,
                    PlayedGameId = playedGame.Id,
                    PlayerId = 1
                });

            playedGame.PlayerGameResults.Add(new PlayerGameResult()
            {
                GameRank = 2,
                GordonPoints = 2,
                Id = 2,
                PlayedGameId = playedGame.Id,
                PlayerId = 2
            });

            playedGame.PlayerGameResults.Add(new PlayerGameResult()
            {
                GameRank = 3,
                GordonPoints = 1,
                Id = 3,
                PlayedGameId = playedGame.Id,
                PlayerId = 3,
                PlayedGame = new PlayedGame() { GameDefinition = new GameDefinition() {  Id = 135, Name = "Test game name"} }
            });

            detailsBuilder = MockRepository.GenerateMock<IGameResultViewModelBuilder>();
            builder = new PlayedGameDetailsViewModelBuilder(detailsBuilder);

            int totalPlayerGameResults = playedGame.PlayerGameResults.Count;
            for (int i = 0; i < totalPlayerGameResults; i++)
            {
                detailsBuilder.Expect(
                    x => x.Build(playedGame.PlayerGameResults[i]))
                    .Repeat
                    .Once()
                    .Return(new GameResultViewModel() { PlayerId = playedGame.PlayerGameResults[i].PlayerId });
            }
            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = gamingGroupId
            };

            playedGameDetails = builder.Build(playedGame, currentUser);
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
            Assert.AreEqual(playedGame.GameDefinitionId, playedGameDetails.GameDefinitionId);
        }

        [Test]
        public void ItCopiesThePlayedGameName()
        {
            Assert.AreEqual(playedGame.GameDefinition.Name, playedGameDetails.GameDefinitionName);
        }

        [Test]
        public void ItCopiesThePlayedGameId()
        {
            Assert.AreEqual(playedGame.Id, playedGameDetails.PlayedGameId);
        }

        [Test]
        public void ItCopiesTheDatePlayed()
        {
            Assert.AreEqual(playedGame.DatePlayed.Ticks, playedGameDetails.DatePlayed.Ticks);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(playedGame.GamingGroup.Id, playedGameDetails.GamingGroupId);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(playedGame.GamingGroup.Name, playedGameDetails.GamingGroupName);
        }

        [Test]
        public void ItCopiesTheNotesReplacingNewlinesWithBreakTags()
        {
            Assert.That(playedGame.Notes.Replace(Environment.NewLine, PlayedGameDetailsViewModelBuilder.NEWLINE_REPLACEMENT_FOR_HTML), Is.EqualTo(playedGameDetails.Notes));
        }

        [Test]
        public void ItTransformsPlayedGameResultsIntoPlayerGameResultSummaries()
        {
            for (int i = 0; i < playedGame.PlayerGameResults.Count; i++)
            {
                Assert.AreEqual(playedGame.PlayerGameResults[i].PlayerId, playedGameDetails.PlayerResults[i].PlayerId);
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
