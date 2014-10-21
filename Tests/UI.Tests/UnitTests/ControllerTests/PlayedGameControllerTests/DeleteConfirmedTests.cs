using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class DeleteConfirmedTests : PlayedGameControllerTestBase
    {
        [Test]
        public void ItDeletesThePlayedGame()
        {
            int playedGameId = 6;
            playedGameController.DeleteConfirmed(playedGameId, currentUser);
            
            playedGameDeleterMock.AssertWasCalled(mock => mock.DeletePlayedGame(playedGameId, currentUser));
        }
    }
}
