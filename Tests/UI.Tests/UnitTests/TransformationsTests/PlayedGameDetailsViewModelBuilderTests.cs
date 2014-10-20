using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class PlayedGameDetailsBuilderTests
    {
        private PlayedGameDetailsViewModelBuilder builder;
        private PlayedGame playedGame;
        private PlayedGameDetailsViewModel playedGameDetails;
        private IGameResultViewModelBuilder detailsBuilder;
        private ApplicationUser currentUser;
        private int gamingGroupId = 1123;

        [SetUp]
        public void SetUp()
        {
            playedGame = new PlayedGame()
            {
                Id = 11111,
                GameDefinition = new GameDefinition(),
                GameDefinitionId = 2222,
                PlayerGameResults = new List<PlayerGameResult>(),
                GamingGroupId = gamingGroupId
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
        public void ItRequiresAGameDefinitionOnThePlayedGame()
        {
            PlayedGame playedGameWithNoGameDefinition = new PlayedGame();

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playedGameWithNoGameDefinition, currentUser)
                );

            Assert.AreEqual(PlayedGameDetailsViewModelBuilder.EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresPlayerGameResultsOnThePlayedGame()
        {
            PlayedGame playedGameWithNoPlayerGameResults = new PlayedGame() { GameDefinition = new GameDefinition() };

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playedGameWithNoPlayerGameResults, currentUser)
                );

            Assert.AreEqual(PlayedGameDetailsViewModelBuilder.EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL, exception.Message);
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
    }
}
