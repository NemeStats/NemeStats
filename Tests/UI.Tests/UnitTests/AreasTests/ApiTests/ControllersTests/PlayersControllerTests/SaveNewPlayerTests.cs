using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayersControllerTests
{
    [TestFixture]
    public class SaveNewPlayerTests : ApiControllerTestBase<PlayersController>
    {
        [Test]
        public void ItSavesTheNewPlayer()
        {
            var newPlayerMessage = new NewPlayerMessage
            {
                PlayerName = "some player name"
            };

            autoMocker.ClassUnderTest.SaveNewPlayer(newPlayerMessage, 0);

            autoMocker.Get<IPlayerSaver>().AssertWasCalled(
                mock => mock.Save(Arg<Player>.Matches(player => player.Name == newPlayerMessage.PlayerName), 
                    Arg<ApplicationUser>.Is.Anything));
        }
    }
}
