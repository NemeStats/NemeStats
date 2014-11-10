using BusinessLogic.Models;
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
        protected GameDefinition gameDefinition;
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
            gameDefinition = new GameDefinition()
            {
                Id = 1,
                Name = "game definition name",
                Description = "game definition description",
                GamingGroupId = gamingGroupid,
                PlayedGames = playedGames
            };
            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = gamingGroupid
            };
            playedGameDetailsViewModelBuilder.Expect(mock => mock.Build(gameDefinition.PlayedGames[0], currentUser))
                .Return(playedGameDetailsViewModel1);
            playedGameDetailsViewModelBuilder.Expect(mock => mock.Build(gameDefinition.PlayedGames[1], currentUser))
                .Return(playedGameDetailsViewModel2);

            viewModel = transformer.Build(gameDefinition, currentUser);
        }

        [Test]
        public void ItCopiesTheId()
        {
            Assert.AreEqual(gameDefinition.Id, viewModel.Id);
        }

        [Test]
        public void ItCopiesTheName()
        {
            Assert.AreEqual(gameDefinition.Name, viewModel.Name);
        }

        [Test]
        public void ItCopiesTheDescription()
        {
            Assert.AreEqual(gameDefinition.Description, viewModel.Description);
        }

        [Test]
        public void ItTransformsThePlayedGamesIntoPlayedGameDetailViewModelsAndSetsOnTheViewModel()
        {
            Assert.AreEqual(playedGameDetailsViewModel1, viewModel.PlayedGames[0]);
            Assert.AreEqual(playedGameDetailsViewModel2, viewModel.PlayedGames[1]);
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            viewModel = transformer.Build(gameDefinition, currentUser);

            Assert.True(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            viewModel = transformer.Build(gameDefinition, currentUser);

            Assert.False(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            viewModel = transformer.Build(gameDefinition, null);

            Assert.False(viewModel.UserCanEdit);
        }
    }
}
