using System.Net.Http;
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
        private Player expectedPlayer;

        [SetUp]
        public void SetUp()
        {
            expectedPlayer = new Player
            {
                Id = 1
            };
            autoMocker.Get<IPlayerSaver>().Expect(mock => mock.Save(null, null)).IgnoreArguments().Return(expectedPlayer);
        }

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

        [Test]
        public void ItReturnsThePlayerIdOfTheNewlyCreatedPlayer()
        {
            var actualResponse = autoMocker.ClassUnderTest.SaveNewPlayer(new NewPlayerMessage(), 0);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewlyCreatedPlayerMessage>)));
            var content = actualResponse.Content as ObjectContent<NewlyCreatedPlayerMessage>;
            var newlyCreatedPlayerMessage = content.Value as NewlyCreatedPlayerMessage;
            Assert.That(newlyCreatedPlayerMessage.PlayerId, Is.EqualTo(expectedPlayer.Id));
        }
    }
}
