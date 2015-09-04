using System.Linq;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayedGamesControllerTests
{
    [TestFixture]
    public class DeletePlayedGameTests : ApiControllerTestBase<PlayedGamesController>
    {
        [Test]
        public void ItDeletesTheSpecifiedPlayedGame()
        {
            const int PLAYED_GAME_ID = 1;

            autoMocker.ClassUnderTest.DeletePlayedGame(PLAYED_GAME_ID, this.applicationUser.CurrentGamingGroupId.Value);

            autoMocker.Get<IPlayedGameDeleter>().AssertWasCalled(mock => mock.DeletePlayedGame(Arg<int>.Is.Equal(PLAYED_GAME_ID), Arg<ApplicationUser>.Is.Same(this.applicationUser)));
        }

        [Test]
        public void ItReturnsANoContentResponse()
        {
            var actualResults = autoMocker.ClassUnderTest.DeletePlayedGame(0, 0);

            AssertThatApiAction.ReturnsANoContentResponseWithNoContent(actualResults);
        }
    }
}
