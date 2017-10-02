using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.Players.PlayerRetrieverTests
{
    [TestFixture]
    public class GetPlayersToCreateIntegrationTests : IntegrationTestIoCBase
    {
        [Test]
        [Category("Integration")]
        public void It_Doesnt_Blow_Up_With_A_Big_Gaming_Group()
        {
            //--arrange
            int minGamingGroupPlayers = 100;
            var dataContext = GetInstance<IDataContext>();
            var bigGamingGroupId =
                dataContext.GetQueryable<GamingGroup>()
                .Where(x => x.Players.Count > minGamingGroupPlayers)
                .Select(x => x.Id)
                .FirstOrDefault();

            //--act
            if (bigGamingGroupId == default(int))
            {
                return;
            }
            var playerRetriever = GetInstance<IPlayerRetriever>();
            var results = playerRetriever.GetPlayersToCreate("anything", bigGamingGroupId);

            //--assert
            results.RecentPlayers.Count.ShouldBe(PlayerRetriever.MAX_NUMBER_OF_RECENT_PLAYERS);
            results.OtherPlayers.Count.ShouldBeGreaterThanOrEqualTo(minGamingGroupPlayers - PlayerRetriever.MAX_NUMBER_OF_RECENT_PLAYERS);
        }

        [Test]
        [Category("Integration")]
        public void It_Doesnt_Blow_Up_With_A_Small_Gaming_Group()
        {
            //--arrange
            var dataContext = GetInstance<IDataContext>();
            var gamingGroupId =
                dataContext.GetQueryable<GamingGroup>()
                .Where(x => x.Players.Count == 1)
                .Select(x => x.Id)
                .FirstOrDefault();

            //--act
            if (gamingGroupId == default(int))
            {
                return;
            }
            var playerRetriever = GetInstance<IPlayerRetriever>();
            var results = playerRetriever.GetPlayersToCreate("anything", gamingGroupId);

            //--assert
            results.RecentPlayers.Count.ShouldBe(1);
            results.OtherPlayers.Count.ShouldBe(0);
        }
    }
}
