#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
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
