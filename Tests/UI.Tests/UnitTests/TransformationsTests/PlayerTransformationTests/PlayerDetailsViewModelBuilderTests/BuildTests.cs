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
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests.PlayerDetailsViewModelBuilderTests
{
    [TestFixture]
    public class BuildTests
    {
        private IGameResultViewModelBuilder gameResultViewModelBuilder;
        private IMinionViewModelBuilder minionViewModelBuilderMock;
        private PlayerDetails playerDetails;
        private PlayerDetailsViewModel playerDetailsViewModel;
        private PlayerDetailsViewModelBuilder builder;
        private ApplicationUser currentUser;

        [SetUp]
        public void TestFixtureSetUp()
        {
            minionViewModelBuilderMock = MockRepository.GenerateMock<IMinionViewModelBuilder>();

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
                NumberOfGamesLost = 3,
                LossPercentage = 75,
                NemesisPlayer = new Player()
                {
                    Name = "Ace Nemesis"
                }
            };

            List<Player> minionPlayers = new List<Player>()
            {
                new Player(){ Id = 1, Nemesis = new Nemesis()},
                new Player(){ Id = 2, Nemesis = new Nemesis()}
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
                PlayerNemesis = nemesis,
                Minions = new List<Player>(),
                GamingGroupId = 123
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
            foreach (Player player in playerDetails.Minions)
            {
                minionViewModelBuilderMock.Expect(mock => mock.Build(player))
                    .Return(new MinionViewModel() { MinionPlayerId = player.Id });
            }

            builder = new PlayerDetailsViewModelBuilder(gameResultViewModelBuilder, minionViewModelBuilderMock);

            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = playerDetails.GamingGroupId
            };

            playerDetailsViewModel = builder.Build(playerDetails, currentUser);
        }

        [Test]
        public void PlayerDetailsCannotBeNull()
        {
            PlayerDetailsViewModelBuilder builder = new PlayerDetailsViewModelBuilder(null, null);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                    builder.Build(null, currentUser));

            Assert.AreEqual("playerDetails", exception.ParamName);
        }

        [Test]
        public void ItRequiresPlayerGameResults()
        {
            PlayerDetailsViewModelBuilder builder = new PlayerDetailsViewModelBuilder(null, null);

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(new PlayerDetails(), currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresPlayerStatistics()
        {
            PlayerDetailsViewModelBuilder builder = new PlayerDetailsViewModelBuilder(null, null);
            PlayerDetails playerDetailsWithNoStatistics = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerDetailsWithNoStatistics, currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void MinionsCannotBeNull()
        {
            PlayerDetailsViewModelBuilder builder = new PlayerDetailsViewModelBuilder(null, null);
            PlayerDetails playerDetailsWithNoMinions = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            playerDetailsWithNoMinions.PlayerStats = new PlayerStatistics();

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerDetailsWithNoMinions, currentUser));

            Assert.AreEqual(PlayerDetailsViewModelBuilder.EXCEPTION_MINIONS_CANNOT_BE_NULL, exception.Message);
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
            Assert.AreEqual(playerDetails.PlayerNemesis.NemesisPlayerId, playerDetailsViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItPopulatesTheNemesisName()
        {
            Assert.AreEqual(playerDetails.PlayerNemesis.NemesisPlayer.Name, playerDetailsViewModel.NemesisName);
        }

        [Test]
        public void ItPopulatesTheGamesLostVersusTheNemesis()
        {
            Assert.AreEqual(playerDetails.PlayerNemesis.NumberOfGamesLost, playerDetailsViewModel.NumberOfGamesLostVersusNemesis);
        }

        [Test]
        public void ItPopulatesTheLostPercentageVersusTheNemesis()
        {
            Assert.AreEqual(playerDetails.PlayerNemesis.LossPercentage, playerDetailsViewModel.LossPercentageVersusPlayer);
        }

        [Test]
        public void ItSetsTheMinions()
        {
            foreach(Player player in playerDetails.Minions)
            {
                Assert.True(playerDetailsViewModel.Minions.Any(minion => minion.MinionPlayerId == player.Id));
            }
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
