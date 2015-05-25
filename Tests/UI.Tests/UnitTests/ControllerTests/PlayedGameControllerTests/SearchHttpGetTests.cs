using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Migrations;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Models.PlayedGame;
using BusinessLogic.Models.Games;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class SearchHttpGetTests : PlayedGameControllerTestBase
    {
        private const int GAME_DEFINITION_ID = 1;
        private const string GAME_DEFINITION_NAME = "some game definition name";

        [SetUp]
        public void SetUp()
        {
            var gameDefinitionNames = new List<GameDefinitionName>
            {
                new GameDefinitionName
                {
                    Name = GAME_DEFINITION_NAME,
                    Id = GAME_DEFINITION_ID
                }
            };
            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitionNames(currentUser)).Return(gameDefinitionNames);

        }

        [Test]
        public void ItReturnsTheCorrectView()
        {
            var actualResults = autoMocker.ClassUnderTest.SearchPlayedGames(currentUser) as ViewResult;

            Assert.That(actualResults.ViewName, Is.EqualTo(MVC.PlayedGame.Views.Search));
        }

        [Test]
        public void ItReturnsTheCorrectViewModelType()
        {
            var actualResults = autoMocker.ClassUnderTest.SearchPlayedGames(currentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel, Is.TypeOf(typeof(SearchViewModel)));
        }

        [Test]
        public void ItShowsAListOfAllGameDefinitionsThisGamingGroupPlaysWithTheEmptyAllOptionSelected()
        {
            var actualResults = autoMocker.ClassUnderTest.SearchPlayedGames(currentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel.GameDefinitions.First(x => x.Value == string.Empty).Text, Is.EqualTo("All"));
            Assert.That(actualViewModel.GameDefinitions.First(x => x.Value == GAME_DEFINITION_ID.ToString(CultureInfo.InvariantCulture)).Text, Is.EqualTo(GAME_DEFINITION_NAME));

        }
    }
}
