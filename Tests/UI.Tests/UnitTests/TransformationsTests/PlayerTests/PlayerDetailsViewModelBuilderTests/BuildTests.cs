using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.Player;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTests.PlayerDetailsViewModelBuilderTests
{
    [TestFixture]
    public class BuildTests
    {
        private PlayerDetails playerDetails;
        private PlayerDetailsViewModel playerDetailsViewModel;
        private PlayerDetailsViewModelBuilder builder;
        private ApplicationUser currentUser;

        [SetUp]
        public void TestFixtureSetUp()
        {
            GameDefinition gameDefinition1 = new GameDefinition()
            {
                Name = "test game 1",
                Id = 1
            };
            PlayedGame playedGame1 = new PlayedGame()
            {
                Id = 1,
                GameDefinition = gameDefinition1
            };
            GameDefinition gameDefinition2 = new GameDefinition()
            {
                Name = "test game 2",
                Id = 2
            };
            PlayedGame playedGame2 = new PlayedGame()
            {
                Id = 2,
                GameDefinition = gameDefinition2
            };
            List<PlayerGameResult> playerGameResults = new List<PlayerGameResult>()
            {
                new PlayerGameResult(){ PlayedGameId = 12, PlayedGame = playedGame1 },
                new PlayerGameResult(){ PlayedGameId = 13, PlayedGame = playedGame2 }
            };

            Nemesis nemesis = new Nemesis() 
            { 
                NemesisPlayerId = 123, 
                GamesLostVersusNemesis = 3, 
                LossPercentageVersusNemesis = 75, 
                NemesisPlayerName = "Ace Nemesis" 
            };

            playerDetails = new PlayerDetails()
            {
                Id = 134,
                Active = true,
                Name = "Skipper",
                PlayerGameResults = playerGameResults,
                PlayerStats = new PlayerStatistics() 
                    { 
                        TotalGames = 71,
                        TotalPoints = 150,
                        AveragePlayersPerGame = 3
                    },
                Nemesis = nemesis,
                GamingGroupId = 123
            };

            IGameResultViewModelBuilder relatedEntityBuilder
                = MockRepository.GenerateMock<IGameResultViewModelBuilder>();
            relatedEntityBuilder.Expect(build => build.Build(playerGameResults[0]))
                    .Repeat
                    .Once()
                    .Return(new Models.PlayedGame.GameResultViewModel() { PlayedGameId = playerGameResults[0].PlayedGameId });
            relatedEntityBuilder.Expect(build => build.Build(playerGameResults[1]))
                    .Repeat
                    .Once()
                    .Return(new Models.PlayedGame.GameResultViewModel() { PlayedGameId = playerGameResults[1].PlayedGameId });
            builder = new PlayerDetailsViewModelBuilder(relatedEntityBuilder);

            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = playerDetails.GamingGroupId
            };

            playerDetailsViewModel = builder.Build(playerDetails, currentUser);
        }

        [Test]
        public void PlayerDetailsCannotBeNull()
        {
            PlayerDetailsViewModelBuilder builder = new PlayerDetailsViewModelBuilder(null);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                    builder.Build(null, currentUser));

            Assert.AreEqual("playerDetails", exception.ParamName);
        }

        [Test]
        public void ItRequiresPlayerGameResults()
        {
            PlayerDetailsViewModelBuilder builder = new PlayerDetailsViewModelBuilder(null);

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(new PlayerDetails(), currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresPlayerStatistics()
        {
            PlayerDetailsViewModelBuilder builder = new PlayerDetailsViewModelBuilder(null);
            PlayerDetails playerDetailsWithNoStatistics = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerDetailsWithNoStatistics, currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Assert.AreEqual(playerDetails.Id, playerDetailsViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            Assert.AreEqual(playerDetails.Name, playerDetailsViewModel.PlayerName);
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
        public void ItCopiesTheTotalPoints()
        {
            Assert.AreEqual(playerDetails.PlayerStats.TotalPoints, playerDetailsViewModel.TotalPoints);
        }

        [Test]
        public void ItSetsTheAveragePointsPerGame()
        {
            float expectedPoints = (float)playerDetails.PlayerStats.TotalPoints / (float)playerDetails.PlayerStats.TotalGames;

            Assert.AreEqual(expectedPoints, playerDetailsViewModel.AveragePointsPerGame);
        }

        [Test]
        public void ItSetsTheAveragePointsPerGameToZeroIfNoGamesHaveBeenPlayed()
        {
            playerDetails.PlayerStats.TotalGames = 0;

            playerDetailsViewModel = builder.Build(playerDetails, currentUser);

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
            float expectedPointsPerGame = (float)playerDetails.PlayerStats.TotalPoints / (float)playerDetails.PlayerStats.TotalGames;
            float expectedPointsPerPlayer = expectedPointsPerGame / (float)playerDetails.PlayerStats.AveragePlayersPerGame;

            Assert.AreEqual(expectedPointsPerPlayer, playerDetailsViewModel.AveragePointsPerPlayer);
        }

        [Test]
        public void ItSetsTheAveragePointsPerPlayerToZeroIfTheAveragePlayersPerGameIsZero()
        {
            playerDetails.PlayerStats.AveragePlayersPerGame = 0;

            PlayerDetailsViewModel viewModel = builder.Build(playerDetails, currentUser);

            Assert.AreEqual(0, viewModel.AveragePointsPerPlayer);
        }

        [Test]
        public void ItPopulatesThePlayerGameSummaries()
        {
            int numberOfPlayerGameResults = playerDetails.PlayerGameResults.Count();
            int expectedPlayedGameId;
            int actualPlayedGameId;
            for(int i = 0; i < numberOfPlayerGameResults; i++)
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
            Assert.AreEqual(playerDetails.Nemesis.NemesisPlayerId, playerDetailsViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItPopulatesTheNemesisName()
        {
            Assert.AreEqual(playerDetails.Nemesis.NemesisPlayerName, playerDetailsViewModel.NemesisName);
        }

        [Test]
        public void ItPopulatesTheGamesLostVersusTheNemesis()
        {
            Assert.AreEqual(playerDetails.Nemesis.GamesLostVersusNemesis, playerDetailsViewModel.NumberOfGamesLostVersusNemesis);
        }

        [Test]
        public void ItPopulatesTheLostPercentageVersusTheNemesis()
        {
            Assert.AreEqual(playerDetails.Nemesis.LossPercentageVersusNemesis, playerDetailsViewModel.LossPercentageVersusPlayer);
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            PlayerDetailsViewModel viewModel = builder.Build(playerDetails, currentUser);

            Assert.True(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            PlayerDetailsViewModel viewModel = builder.Build(playerDetails, currentUser);

            Assert.False(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            PlayerDetailsViewModel viewModel = builder.Build(playerDetails, null);

            Assert.False(viewModel.UserCanEdit);
        }
    }
}
