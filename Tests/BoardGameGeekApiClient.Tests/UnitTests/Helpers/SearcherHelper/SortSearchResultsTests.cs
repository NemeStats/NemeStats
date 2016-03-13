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

using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using BoardGameGeekApiClient.Models;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.SearcherHelper
{
    [TestFixture]
    public class SortSearchResultsTests
    {
        private const string TOSHO = "Tosho";
        private List<SearchBoardGameResult> startingResults;
        private List<string> expectedOrder;
        private List<SearchBoardGameResult> actualResults;

        [SetUp]
        public virtual void SetUp()
        {
            expectedOrder = new List<string>
            {
                TOSHO,
                TOSHO + " is amazing",
                TOSHO + " is the man and he likes to be hugged",
                "Everyone knows " + TOSHO + " is awesome"
            };
            startingResults = new List<SearchBoardGameResult>
            {
                new SearchBoardGameResult
                {
                    BoardGameName = expectedOrder[2]
                },
                new SearchBoardGameResult
                {
                    BoardGameName = expectedOrder[3]
                },
                new SearchBoardGameResult
                {
                    BoardGameName = expectedOrder[0]
                },
                new SearchBoardGameResult
                {
                    BoardGameName = expectedOrder[1]
                }
            };


        }

        public class When_There_Are_A_Limit : SortSearchResultsTests
        {
            private int limit = 1;
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                actualResults = startingResults.SortSearchResults(TOSHO, maxResultsToReturn: limit).ToList();
            }

            [Test]
            public void Then_Returns_Limited_Number_Of_Elements()
            {
                Assert.AreEqual(limit, actualResults.Count);
            }
        }

        public class When_There_Are_Not_Limit : SortSearchResultsTests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                actualResults = startingResults.SortSearchResults(TOSHO).ToList();
            }

            [Test]
            public void ItPutsExactMatchesFirst()
            {
                Assert.AreEqual(expectedOrder[0], actualResults[0].BoardGameName);
            }

            [Test]
            public void ItPutsTheShortestStartsWithMatchesSecond()
            {
                Assert.AreEqual(expectedOrder[1], actualResults[1].BoardGameName);
            }

            [Test]
            public void ItPutsOtherStartsWithMatchesNext()
            {
                Assert.AreEqual(expectedOrder[2], actualResults[2].BoardGameName);
            }

            [Test]
            public void ItPutsOtherMatchesLast()
            {
                Assert.AreEqual(expectedOrder[3], actualResults[3].BoardGameName);
            }
        }


    }
}
