using System.Net;
using System.Net.Http;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayersControllerTests
{
    [TestFixture]
    public class UpdatePlayerTests : ApiControllerTestBase<PlayersController>
    {
        [Test]
        public void ItSavesThePlayer()
        {
            const int PLAYER_ID = 1;

            var updatePlayerMessage = new UpdatePlayerMessage
            {
                PlayerName = "some player name",
                Active = false
            };

            _autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerMessage, PLAYER_ID, 100);

            _autoMocker.Get<IPlayerSaver>().AssertWasCalled(mock => mock.UpdatePlayer(
                Arg<UpdatePlayerRequest>.Matches(player => player.Active == updatePlayerMessage.Active
                                    && player.Name == updatePlayerMessage.PlayerName
                                    && player.PlayerId == PLAYER_ID),
                Arg<ApplicationUser>.Is.Same(_applicationUser)));
        }

        [Test]
        public void ItReturnsABadRequestIfTheMessageIsNull()
        {
            HttpResponseMessage actualResponse = _autoMocker.ClassUnderTest.UpdatePlayer(null, 1, 100);

            AssertThatApiAction.HasThisError(actualResponse, HttpStatusCode.BadRequest, "You must pass at least one valid parameter.");
        }

        [Test]
        public void ItReturnsANoContentResponse()
        {
            var actualResults = _autoMocker.ClassUnderTest.UpdatePlayer(new UpdatePlayerMessage(), 0, 0);

            AssertThatApiAction.ReturnsANoContentResponseWithNoContent(actualResults);
        }
    }
}
