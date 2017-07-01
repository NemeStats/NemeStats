using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Models.PlayedGame;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class SearchHttpGetTests : PlayedGameControllerTestBase
    {
        private const int GAME_DEFINITION_B_ID = 1;
        private const string GAME_DEFINITION_NAME_B = "B - some game definition name";
        private const int GAME_DEFINITION_A_ID = 2;
        private const string GAME_DEFINITION_NAME_A = "A - some game definition name";

        [SetUp]
        public void SetUp()
        {
            var gameDefinitionNames = new List<GameDefinitionName>
            {
                new GameDefinitionName
                {
                    Name = GAME_DEFINITION_NAME_B,
                    Id = GAME_DEFINITION_B_ID
                },
                new GameDefinitionName
                {
                    Name = GAME_DEFINITION_NAME_A,
                    Id = GAME_DEFINITION_A_ID
                }
            };
            AutoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitionNames(this.CurrentUser.CurrentGamingGroupId)).Return(gameDefinitionNames);

            var players = new List<Player>
            {
                new Player
                {
                    Name = "Test Player",
                    Id = 1
                }
            };
            AutoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetAllPlayers(this.CurrentUser.CurrentGamingGroupId, false)).Return(players);
        }

        [Test]
        public void ItReturnsTheCorrectView()
        {
            var actualResults = AutoMocker.ClassUnderTest.Search(CurrentUser) as ViewResult;

            Assert.That(actualResults.ViewName, Is.EqualTo(MVC.PlayedGame.Views.Search));
        }

        [Test]
        public void ItReturnsTheCorrectViewModelType()
        {
            var actualResults = AutoMocker.ClassUnderTest.Search(CurrentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel, Is.TypeOf(typeof(SearchViewModel)));
        }

        [Test]
        public void ItShowsAListOfAllGameDefinitionsThisGamingGroupPlaysWithTheEmptyAllOptionSelected()
        {
            var actualResults = AutoMocker.ClassUnderTest.Search(CurrentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel.GameDefinitions.First(x => x.Value == string.Empty).Text, Is.EqualTo("All"));
            Assert.That(actualViewModel.GameDefinitions.First(x => x.Value == GAME_DEFINITION_B_ID.ToString(CultureInfo.InvariantCulture)).Text, Is.EqualTo(GAME_DEFINITION_NAME_B));
            Assert.That(actualViewModel.GameDefinitions.First(x => x.Value == GAME_DEFINITION_A_ID.ToString(CultureInfo.InvariantCulture)).Text, Is.EqualTo(GAME_DEFINITION_NAME_A));
        }

        [Test]
        public void ItListsGameDefinitionsInAlphabeticalOrderWithTheAllOptionListedFirst()
        {
            var actualResults = AutoMocker.ClassUnderTest.Search(CurrentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel.GameDefinitions[0].Text, Is.EqualTo("All"));
            Assert.That(actualViewModel.GameDefinitions[1].Text, Is.EqualTo(GAME_DEFINITION_NAME_A));
            Assert.That(actualViewModel.GameDefinitions[2].Text, Is.EqualTo(GAME_DEFINITION_NAME_B));
        }
    }
}
