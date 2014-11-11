using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GameDefinitionViewModelBuilderTests
    {
        protected GameDefinitionViewModelBuilder transformer;
        protected IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;
        protected GameDefinitionSummary gameDefinitionSummary;
        protected GameDefinitionViewModel viewModel;
        protected PlayedGameDetailsViewModel playedGameDetailsViewModel1;
        protected PlayedGameDetailsViewModel playedGameDetailsViewModel2;
        protected ApplicationUser currentUser;
        protected int gamingGroupid = 135;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            playedGameDetailsViewModelBuilder = MockRepository.GenerateMock<IPlayedGameDetailsViewModelBuilder>();
            transformer = new GameDefinitionViewModelBuilder(playedGameDetailsViewModelBuilder);            

            List<PlayedGame> playedGames = new List<PlayedGame>();
            playedGames.Add(new PlayedGame()
                {
                    Id = 10
                });
            playedGameDetailsViewModel1 = new PlayedGameDetailsViewModel();
            playedGames.Add(new PlayedGame()
            {
                Id = 11
            });
            playedGameDetailsViewModel2 = new PlayedGameDetailsViewModel();
            gameDefinitionSummary = new GameDefinitionSummary()
            {
                Id = 1,
                Name = "game definition name",
                Description = "game definition description",
                GamingGroupId = gamingGroupid,
                GamingGroupName = "gaming group name",
                PlayedGames = playedGames
            };
            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = gamingGroupid
            };
            playedGameDetailsViewModelBuilder.Expect(mock => mock.Build(gameDefinitionSummary.PlayedGames[0], currentUser))
                .Return(playedGameDetailsViewModel1);
            playedGameDetailsViewModelBuilder.Expect(mock => mock.Build(gameDefinitionSummary.PlayedGames[1], currentUser))
                .Return(playedGameDetailsViewModel2);

            viewModel = transformer.Build(gameDefinitionSummary, currentUser);
        }

        [Test]
        public void ItCopiesTheId()
        {
            Assert.AreEqual(gameDefinitionSummary.Id, viewModel.Id);
        }

        [Test]
        public void ItCopiesTheName()
        {
            Assert.AreEqual(gameDefinitionSummary.Name, viewModel.Name);
        }

        [Test]
        public void ItCopiesTheDescription()
        {
            Assert.AreEqual(gameDefinitionSummary.Description, viewModel.Description);
        }

        [Test]
        public void ItCopiesTheTotalNumberOfGamesPlayed()
        {
            Assert.AreEqual(gameDefinitionSummary.TotalNumberOfGamesPlayed, viewModel.TotalNumberOfGamesPlayed);
        }

        [Test]
        public void ItTransformsThePlayedGamesIntoPlayedGameDetailViewModelsAndSetsOnTheViewModel()
        {
            Assert.AreEqual(playedGameDetailsViewModel1, viewModel.PlayedGames[0]);
            Assert.AreEqual(playedGameDetailsViewModel2, viewModel.PlayedGames[1]);
        }

        [Test]
        public void ItSetsThePlayedGamesToAnEmptyListIfThereAreNone()
        {
            gameDefinitionSummary.PlayedGames = null;

            GameDefinitionViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.AreEqual(new List<PlayedGameDetailsViewModel>(), actualViewModel.PlayedGames);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(gameDefinitionSummary.GamingGroupName, viewModel.GamingGroupName);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(gameDefinitionSummary.GamingGroupId, viewModel.GamingGroupId);
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            GameDefinitionViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.True(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            GameDefinitionViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            GameDefinitionViewModel actualViewModel = transformer.Build(gameDefinitionSummary, null);

            Assert.False(actualViewModel.UserCanEdit);
        }
    }
}
