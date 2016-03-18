using System.Net.Http;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
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
        private Player _expectedPlayer;
        private readonly int _expectedGamingGroupId = 2;

        [SetUp]
        public void SetUp()
        {
            _expectedPlayer = new Player
            {
                Id = 1,
                GamingGroupId = _expectedGamingGroupId
            };
            _autoMocker.Get<IPlayerSaver>().Expect(mock => mock.CreatePlayer(
                Arg<CreatePlayerRequest>.Is.Anything, 
                Arg<ApplicationUser>.Is.Anything,
                Arg<bool>.Is.Anything)).IgnoreArguments().Return(_expectedPlayer);
        }

        [Test]
        public void ItSavesTheNewPlayer()
        {
            var newPlayerMessage = new NewPlayerMessage
            {
                PlayerName = "some player name",
                GamingGroupId = _expectedGamingGroupId
            };

            _autoMocker.ClassUnderTest.SaveNewPlayer(newPlayerMessage, _expectedGamingGroupId);

            _autoMocker.Get<IPlayerSaver>().AssertWasCalled(
                mock => mock.CreatePlayer(Arg<CreatePlayerRequest>.Matches(player => player.Name == newPlayerMessage.PlayerName
                && player.GamingGroupId == _expectedGamingGroupId),
                    Arg<ApplicationUser>.Is.Anything,
                    Arg<bool>.Is.Equal(false)));
        }

        [Test]
        public void ItSavesTheNewPlayerUsingTheGamingGroupIdOnTheRequestInsteadOfFromTheUriIfOneIsSpecified ()
        {
            var newPlayerMessage = new NewPlayerMessage
            {
                PlayerName = "some player name",
                GamingGroupId = _expectedGamingGroupId
            };
            int someGamingGroupIdThatWontGetUsed = -100;

            _autoMocker.ClassUnderTest.SaveNewPlayer(newPlayerMessage, someGamingGroupIdThatWontGetUsed);

            _autoMocker.Get<IPlayerSaver>().AssertWasCalled(
                mock => mock.CreatePlayer(Arg<CreatePlayerRequest>.Matches(player => player.Name == newPlayerMessage.PlayerName
                && player.GamingGroupId == _expectedGamingGroupId),
                    Arg<ApplicationUser>.Is.Anything,
                    Arg<bool>.Is.Anything));
        }

        [Test]
        public void ItReturnsThePlayerIdAndGamingGroupOfTheNewlyCreatedPlayer()
        {
            var actualResponse = _autoMocker.ClassUnderTest.SaveNewPlayer(new NewPlayerMessage(), _expectedGamingGroupId);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewlyCreatedPlayerMessage>)));
            var content = actualResponse.Content as ObjectContent<NewlyCreatedPlayerMessage>;
            var newlyCreatedPlayerMessage = content.Value as NewlyCreatedPlayerMessage;
            Assert.That(newlyCreatedPlayerMessage.PlayerId, Is.EqualTo(_expectedPlayer.Id));
            Assert.That(newlyCreatedPlayerMessage.GamingGroupId, Is.EqualTo(_expectedGamingGroupId));
        }
    }
}
