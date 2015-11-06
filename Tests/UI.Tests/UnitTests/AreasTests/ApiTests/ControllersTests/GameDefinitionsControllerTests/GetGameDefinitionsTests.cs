using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Net;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.GameDefinitionsControllerTests
{
    [TestFixture]
    public class GetGameDefinitionsTests : ApiControllerTestBase<GameDefinitionsController>
    {
        [Test]
        public void ItReturnsAllTheGameDefinitionsForTheGivenGamingGroup()
        {
            const int GAMING_GROUP_ID = 1;

            var expectedResults = new List<GameDefinitionSummary> 
            {
                new GameDefinitionSummary
                {
                    Id = 2,
                    Name = "some game definition name",
                    Active = false,
                    BoardGameGeekGameDefinitionId = 3
                }
            };

            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitions(GAMING_GROUP_ID)).Return(expectedResults);

            var actualResults = autoMocker.ClassUnderTest.GetGameDefinitions(GAMING_GROUP_ID);

            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<GameDefinitionsSearchResultsMessage>(actualResults, HttpStatusCode.OK);
            var firstActualGameDefinitionSearchResultMessage = actualData.GameDefinitions[0];
            Assert.That(firstActualGameDefinitionSearchResultMessage.GameDefinitionId, Is.EqualTo(expectedResults[0].Id));
            Assert.That(firstActualGameDefinitionSearchResultMessage.GameDefinitionName, Is.EqualTo(expectedResults[0].Name));
            Assert.That(firstActualGameDefinitionSearchResultMessage.Active, Is.EqualTo(expectedResults[0].Active));
            Assert.That(firstActualGameDefinitionSearchResultMessage.BoardGameGeekGameDefinitionId, Is.EqualTo(expectedResults[0].BoardGameGeekGameDefinitionId));

        }
    }
}
