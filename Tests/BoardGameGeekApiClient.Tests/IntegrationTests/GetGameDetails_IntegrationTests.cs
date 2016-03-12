using BoardGameGeekApiClient.Models;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    [TestFixture]
    public class GetGameDetails_IntegrationTests : BaseBoardGameGeekApiClientIntegrationTest
    {
        GameDetails _result;
        public int GameId { get; set; }

        public class When_GameId_Exists : GetGameDetails_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                GameId = 2536;
                _result = ApiClient.GetGameDetails(GameId);
            }

            [Test]
            public void Then_Return_GameDetails()
            {
                Assert.IsNotNull(_result);
                Assert.That(_result.Name, Is.Not.Null);
                Assert.That(_result.Name, Is.Not.Empty);
                Assert.That(_result.AverageWeight, Is.Not.Null);
            }

            [Test]
            public void Then_GameId_Matches_The_Queried_Id()
            {
                Assert.AreEqual(_result.GameId, GameId);
            }
        }

        public class When_GameId_Not_Exists : GetGameDetails_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                GameId = int.MaxValue;
                _result = ApiClient.GetGameDetails(GameId);
            }

            [Test]
            public void Then_Return_Null()
            {
                Assert.IsNull(_result);
            }

        }
    }
}