using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerSaverTests
{
    public class UpdatePlayerTests : PlayerSaverTestBase
    {
        private const int PLAYER_ID = 1;
        private Player player;

        [SetUp]
        public void SetUp()
        {
            player = new Player
            {
                Id = PLAYER_ID,
                Active = true,
                Name = "some existing name"
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(PLAYER_ID)).Return(player);

            autoMocker.PartialMockTheClassUnderTest();
        }

        [Test]
        public void ItUpdatesTheActiveFlag()
        {
            var updatePlayerRequest = new UpdatePlayerRequest
            {
                Active = true,
                PlayerId = PLAYER_ID
            };

            autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerRequest, currentUser);

            autoMocker.ClassUnderTest.AssertWasCalled(
                mock => mock.Save(Arg<Player>.Matches(p => p.Active == updatePlayerRequest.Active.Value),
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItUpdatesThePlayerName()
        {
            var updatePlayerRequest = new UpdatePlayerRequest
            {
                Name = "a new name",
                PlayerId = PLAYER_ID
            };

            autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerRequest, currentUser);

            autoMocker.ClassUnderTest.AssertWasCalled(
                mock => mock.Save(Arg<Player>.Matches(p => p.Name == updatePlayerRequest.Name),
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItDoesntBotherUpdatingIfNothingWasRequestedToBeUpdated()
        {
            var updatePlayerRequest = new UpdatePlayerRequest
            {
                Name = null,
                PlayerId = PLAYER_ID,
                Active = null
            };

            autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerRequest, currentUser);

            autoMocker.ClassUnderTest.AssertWasNotCalled(
                mock => mock.Save(Arg<Player>.Is.Anything,
                    Arg<ApplicationUser>.Is.Same(currentUser)));
        }
    }
}
