using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class ValidateAccessToPlayersTests : PlayedGameSaverTestBase
    {
        [Test]
        public void ItChecksSecurityOnThePlayerId()
        {
            //--arrange
            var playerRanks = new List<PlayerRank>
            {
                new PlayerRank
                {
                    PlayerId = ExistingPlayerWithMatchingGamingGroup.Id
                }
            };

            //--act
            AutoMocker.ClassUnderTest.ValidateAccessToPlayers(playerRanks, GAMING_GROUP_ID, CurrentUser, AutoMocker.Get<IDataContext>());

            //--assert
            AutoMocker.Get<ISecuredEntityValidator>().AssertWasCalled(mock => mock.ValidateAccess(
                ExistingPlayerWithMatchingGamingGroup,
                CurrentUser));
        }
    }
}
