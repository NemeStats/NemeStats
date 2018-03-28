using System.Collections.Generic;
using System.Net;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.HtmlHelpers;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayersControllerTests
{
    [TestFixture]
    public class GetPlayersTests : ApiControllerTestBase<PlayersController>
    {
        [Test]
        public void ItReturnsThePlayers()
        {
            const int GAMING_GROUP_ID = 1;

            var expectedPlayer = new Player
            {
                Id = 2,
                Name = "player 1",
                Active = false,
                NemesisId = 300
            };

            var expectedPlayers = new List<Player>
            {
                expectedPlayer
            };
            _autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetAllPlayers(GAMING_GROUP_ID)).Return(expectedPlayers);

            var actualResults = _autoMocker.ClassUnderTest.GetPlayers(GAMING_GROUP_ID);

            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<PlayersSearchResultsMessage>(actualResults, HttpStatusCode.OK);
            Assert.That(actualData.Players.Count, Is.EqualTo(1));
            var player = actualData.Players[0];
            Assert.That(player.PlayerId, Is.EqualTo(expectedPlayer.Id));
            Assert.That(player.PlayerName, Is.EqualTo(expectedPlayer.Name));
            Assert.That(player.Active, Is.EqualTo(expectedPlayer.Active));
            Assert.That(player.CurrentNemesisPlayerId, Is.EqualTo(expectedPlayer.NemesisId));
            Assert.That(player.NemeStatsUrl, Is.EqualTo(AbsoluteUrlBuilder.GetPlayerDetailsUrl(player.PlayerId)));
        }
    }
}
