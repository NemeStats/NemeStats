using BoardGameGeekApiClient.Models;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    [TestFixture]
    public class GetUserDetails_IntegrationTests : BaseBoardGameGeekApiClientIntegrationTest
    {
        UserDetails _result;
        public string UserName { get; set; }

        public class When_UserName_Exists : GetUserDetails_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                UserName = "kairos";
                _result = ApiClient.GetUserDetails(UserName);
            }

            [Test]
            public void Then_Return_UserDetails()
            {
                Assert.IsNotNull(_result);
                Assert.That(_result.Name, Is.Not.Null);
                Assert.That(_result.Name, Is.Not.Empty);
            }

            [Test]
            public void Then_UserName_Matches_The_Queried_Id()
            {
                Assert.AreEqual(_result.Name, UserName);
            }
        }

        public class When_UserName_Not_Exists : GetUserDetails_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                UserName = "foo/foo";
                _result = ApiClient.GetUserDetails(UserName);
            }

            [Test]
            public void Then_Return_Null()
            {
                Assert.IsNull(_result);
            }

        }
    }
}