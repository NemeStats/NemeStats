using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Models;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    [TestFixture]
    public class SearchBoardGames_IntegrationTests : BaseBoardGameGeekApiClientIntegrationTest
    {
        IEnumerable<SearchBoardGameResult> _result;
        public string Query { get; set; }

        public class When_Query_Match_A_Game : SearchBoardGames_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                Query = "alchemist";
                _result = ApiClient.SearchBoardGames(Query);
            }

            [Test]
            public void Then_Return_Data()
            {
                Assert.IsNotNull(_result);
                Assert.IsNotEmpty(_result);
            }

            [Test]
            public void Then_Data_Matches_The_Query()
            {
                var firstResult = _result.First();

                Assert.IsTrue(firstResult.BoardGameName.ToLower().Contains(Query));
            }
        }

        public class When_Query_Not_Match_A_Game : SearchBoardGames_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                Query = "this game does not exists (or i least i hope so). now a random string: asdiasjdkoashdfkawhfi89wfy234ukzc&";
                _result = ApiClient.SearchBoardGames(Query);
            }

            [Test]
            public void Then_Return_Empty_Array()
            {
                Assert.IsNotNull(_result);
                Assert.IsEmpty(_result);
            }

        }

    }
}
