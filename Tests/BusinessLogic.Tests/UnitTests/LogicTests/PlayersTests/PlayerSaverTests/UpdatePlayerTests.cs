using System.Collections.Generic;
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
        private Player _player;

        [SetUp]
        public void SetUp()
        {
            _player = new Player
            {
                Id = PLAYER_ID,
                Active = true,
                Name = "some existing name",
                ChampionedGames = new List<Champion>
                {
                    new Champion
                    {
                        GameDefinitionId = 10
                    },
                    new Champion
                    {
                        GameDefinitionId = 11
                    }
                }
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(PLAYER_ID)).Return(_player);

            _autoMocker.PartialMockTheClassUnderTest();

            _autoMocker.ClassUnderTest
                .Expect(mock => mock.Save(Arg<Player>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheActiveFlagIfItChanged()
        {
            var updatePlayerRequest = new UpdatePlayerRequest
            {
                Active = !_player.Active,
                PlayerId = PLAYER_ID
            };

            _autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerRequest, _currentUser);

            _autoMocker.ClassUnderTest.AssertWasCalled(
                mock => mock.Save(Arg<Player>.Matches(p => p.Active == updatePlayerRequest.Active.Value),
                    Arg<ApplicationUser>.Is.Same(_currentUser)));
        }

        [Test]
        public void ItUpdatesThePlayerNameIfItChanged()
        {
            var updatePlayerRequest = new UpdatePlayerRequest
            {
                Name = "some new player name",
                PlayerId = PLAYER_ID
            };

            _autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerRequest, _currentUser);

            _autoMocker.ClassUnderTest.AssertWasCalled(
                mock => mock.Save(Arg<Player>.Matches(p => p.Name == updatePlayerRequest.Name),
                    Arg<ApplicationUser>.Is.Same(_currentUser)));
        }

        [Test]
        public void ItDoesntBotherUpdatingIfNothingChanged()
        {
            var updatePlayerRequest = new UpdatePlayerRequest
            {
                Active = _player.Active,
                Name = _player.Name,
                PlayerId = PLAYER_ID
            };

            _autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerRequest, _currentUser);

            _autoMocker.ClassUnderTest.AssertWasNotCalled(
                mock => mock.Save(Arg<Player>.Is.Anything,
                    Arg<ApplicationUser>.Is.Anything));
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

            _autoMocker.ClassUnderTest.UpdatePlayer(updatePlayerRequest, _currentUser);

            _autoMocker.ClassUnderTest.AssertWasNotCalled(
                mock => mock.Save(Arg<Player>.Is.Anything,
                    Arg<ApplicationUser>.Is.Same(_currentUser)));
        }
    }
}
