using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class SearchBoardGameGeekHttpGetTests : GameDefinitionControllerTestBase
    {
        private List<BoardGameGeekSearchResult> expectedSearchResults;
        private string searchText = "game name";
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            expectedSearchResults = new List<BoardGameGeekSearchResult>();
            boardGameGeekSearcherMock.Expect(mock => mock.SearchForBoardGames(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                                     .Return(expectedSearchResults);
        }

        [Test]
        public void ItReturnsBoardGameGeekResultsThatDontHaveToBeExactMatchesWhenFiveOrMoreCharactersAreEntered()
        {
            string fiveCharacterSearchText = "12345";
            gameDefinitionControllerPartialMock.SearchBoardGameGeekHttpGet(fiveCharacterSearchText);

            boardGameGeekSearcherMock.AssertWasCalled(
                mock => mock.SearchForBoardGames(Arg<string>.Is.Equal(fiveCharacterSearchText), Arg<bool>.Is.Equal(false)));
        }

        [Test]
        public void ItRequestsExactMatchesWhenLessThanTheDesignatedNumberOfCharacters()
        {
            string searchString = string.Empty.PadRight(GameDefinitionController.MIN_LENGTH_FOR_PARTIAL_MATCH_BOARD_GAME_GEEK_SEARCH - 1);
            gameDefinitionControllerPartialMock.SearchBoardGameGeekHttpGet(searchString);

            boardGameGeekSearcherMock.AssertWasCalled(
                mock => mock.SearchForBoardGames(Arg<string>.Is.Equal(searchString), Arg<bool>.Is.Equal(true)));
        }
    }
}
