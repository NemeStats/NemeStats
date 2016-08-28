using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.Transformations;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayerStatsControllerTests
{
    [TestFixture]
    public class GetPlayerStatsTests : ApiControllerTestBase<PlayerStatsController>
    {
        private const int PLAYER_ID = 1;
        private PlayerStatistics expectedPlayerStatistics;
        private PlayerStatisticsMessage expectedMessage;

        [SetUp]
        public void SetUp()
        {
            expectedPlayerStatistics = new PlayerStatistics
            {
                GameDefinitionTotals = new GameDefinitionTotals()
            };
            _autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerStatistics(PLAYER_ID)).Return(expectedPlayerStatistics);

            expectedMessage = new PlayerStatisticsMessage();
            _autoMocker.Get<ITransformer>().Expect(
                       mock => mock.Transform<PlayerStatisticsMessage>(expectedPlayerStatistics))
                      .Return(expectedMessage);
        }

        [Test]
        public void ItReturnsAPlayerStatisticsMessageForTheGivenPlayer()
        {
            var results = _autoMocker.ClassUnderTest.GetPlayerStats(0, PLAYER_ID);

            var model = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<PlayerStatisticsMessage>(results, HttpStatusCode.OK);
            Assert.That(model, Is.SameAs(expectedMessage));
        }
    }
}
