using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GameDefinitionToGameDefinitionViewModelTransformationImplTests
    {
        protected GameDefinitionToGameDefinitionViewModelTransformationImpl transformer;
        protected PlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;
        protected GameDefinition gameDefinition;
        protected GameDefinitionViewModel viewModel;
        protected PlayedGameDetailsViewModel playedGameDetailsViewModel1;
        protected PlayedGameDetailsViewModel playedGameDetailsViewModel2;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            playedGameDetailsViewModelBuilder = MockRepository.GenerateMock<PlayedGameDetailsViewModelBuilder>();
            transformer = new GameDefinitionToGameDefinitionViewModelTransformationImpl(playedGameDetailsViewModelBuilder);            

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
                GamingGroupId = 2,
                PlayedGames = playedGames
            };
            playedGameDetailsViewModelBuilder.Expect(mock => mock.Build(gameDefinition.PlayedGames[0]))
                .Return(playedGameDetailsViewModel1);
            playedGameDetailsViewModelBuilder.Expect(mock => mock.Build(gameDefinition.PlayedGames[1]))
                .Return(playedGameDetailsViewModel2);

            viewModel = transformer.Build(gameDefinition);
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
    }
}
