using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models.Games;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekSearcherTests
{
    [TestFixture]
    public class SortSearchResultsTests
    {
        private const string TOSHO = "Tosho";
        private List<BoardGameGeekSearchResult> startingResults;
        private List<string> expectedOrder;
        private List<BoardGameGeekSearchResult> actualResults;
        
        [SetUp]
        public void SetUp()
        {
            expectedOrder = new List<string>
            {
                TOSHO,
                TOSHO + " is the man and he likes to be hugged",
                "Everyone knows " + TOSHO + " is awesome"
            };
            startingResults = new List<BoardGameGeekSearchResult>
            {
                new BoardGameGeekSearchResult
                {
                    BoardGameName = expectedOrder[1]
                },
                new BoardGameGeekSearchResult
                {
                    BoardGameName = expectedOrder[2]
                },
                new BoardGameGeekSearchResult
                {
                    BoardGameName = expectedOrder[0]
                }
            };

            actualResults = BoardGameGeekSearcher.SortSearchResults(TOSHO, startingResults);
        }

        [Test]
        public void ItPutsExactMatchesFirst()
        {
            Assert.AreEqual(expectedOrder[0], actualResults[0].BoardGameName);
        }

        [Test]
        public void ItPutsStartsWithMatchesSecond()
        {
            Assert.AreEqual(expectedOrder[1], actualResults[1].BoardGameName);
        }

        [Test]
        public void ItPutsOtherMatchesLast()
        {
            Assert.AreEqual(expectedOrder[2], actualResults[2].BoardGameName);
        }
    }
}
