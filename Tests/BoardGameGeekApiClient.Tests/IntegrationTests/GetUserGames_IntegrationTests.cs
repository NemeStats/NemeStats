using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Models;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    public class GetUserGames_IntegrationTests : BaseBoardGameGeekApiClientIntegrationTest
    {
        List<GameDetails> _results;
        public string UserName { get; set; }

        public class When_UserName_Exists : GetUserGames_IntegrationTests
        {
            [SetUp]
            public void SetUp()
            {
                UserName = "HolisticDeveloper";
                _results = ApiClient.GetUserGames(UserName);
            }

            [Test]
            public void Then_Return_Games()
            {
                Assert.IsNotNull(_results);
                Assert.IsNotEmpty(_results);
                Assert.That(_results.Exists(g => g.Name == "Too Many Bones"), Is.True);
            }
        }

        public class When_UserName_Not_Exists : GetUserGames_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                UserName = "foo/foo";
                _results = ApiClient.GetUserGames(UserName);
            }

            [Test]
            public void Then_Return_Null()
            {
                Assert.IsEmpty(_results);
            }

        }
    }
}