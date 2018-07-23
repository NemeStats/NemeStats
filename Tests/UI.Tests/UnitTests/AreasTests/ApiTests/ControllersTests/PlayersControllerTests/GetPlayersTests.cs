using System.Collections.Generic;
using System.Net;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.HtmlHelpers;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayersControllerTests
{
    [TestFixture]
    public class GetPlayersTests : ApiControllerTestBase<PlayersController>
    {
        private const int GAMING_GROUP_ID = 1;
        private Player _expectedPlayer;
        private Dictionary<int, string> _expectedDictionary;
        private string _expectedEmail = "some email";

        [SetUp]
        public override void BaseSetUp()
        {
            base.BaseSetUp();
            _expectedPlayer = new Player
            {
                Id = 2,
                Name = "player 1",
                Active = false,
                NemesisId = 300
            };

            var expectedPlayers = new List<Player>
            {
                _expectedPlayer
            };
            _autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetAllPlayers(GAMING_GROUP_ID)).Return(expectedPlayers);

            _expectedDictionary = new Dictionary<int, string>
            {
                {
                    _expectedPlayer.Id, _expectedEmail
                }
            };

            _autoMocker.Get<IPlayerRetriever>().Expect(mock =>
                    mock.GetRegisteredUserEmailAddresses(Arg<IList<int>>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedDictionary);
        }

        [Test]
        public void It_Returns_The_Players()
        {
            var actualResults = _autoMocker.ClassUnderTest.GetPlayers(GAMING_GROUP_ID);

            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<PlayersSearchResultsMessage>(actualResults, HttpStatusCode.OK);
            Assert.That(actualData.Players.Count, Is.EqualTo(1));
            var player = actualData.Players[0];
            Assert.That(player.PlayerId, Is.EqualTo(_expectedPlayer.Id));
            Assert.That(player.PlayerName, Is.EqualTo(_expectedPlayer.Name));
            Assert.That(player.Active, Is.EqualTo(_expectedPlayer.Active));
            Assert.That(player.CurrentNemesisPlayerId, Is.EqualTo(_expectedPlayer.NemesisId));
            Assert.That(player.NemeStatsUrl, Is.EqualTo(AbsoluteUrlBuilder.GetPlayerDetailsUrl(player.PlayerId)));
        }

        [Test]
        public void It_Returns_Gravatar_Urls_For_Registered_Users_The_User_Has_Access_To_See()
        {
            //--arrange
            var expectedGravatarUrl = UIHelper.BuildGravatarUrl(_expectedEmail);

            //--act
            var actualResults = _autoMocker.ClassUnderTest.GetPlayers(GAMING_GROUP_ID);

            //--assert
            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<PlayersSearchResultsMessage>(actualResults, HttpStatusCode.OK);
            actualData.Players.ShouldContain(x =>
                x.PlayerId == _expectedPlayer.Id && x.RegisteredUserGravatarUrl == expectedGravatarUrl);
        }
    }
}
