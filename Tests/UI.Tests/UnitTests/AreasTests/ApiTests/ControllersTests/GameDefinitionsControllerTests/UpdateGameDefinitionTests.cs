using System.Linq;
using System.Net;
using System.Net.Http;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.GameDefinitionsControllerTests
{
    [TestFixture]
    public class UpdateGameDefinitionTests : ApiControllerTestBase<GameDefinitionsController>
    {
        [Test]
        public void ItReturnsABadRequestIfTheMessageIsNull()
        {
            HttpResponseMessage actualResponse = autoMocker.ClassUnderTest.UpdateGameDefinition(null, 0, 0);

            AssertThatApiAction.HasThisError(actualResponse, HttpStatusCode.BadRequest, "You must pass at least one valid parameter.");
        }

        [Test]
        public void ItUpdatesTheGameDefinition()
        {
            const int GAME_DEFINITION_ID = 5;
            const int GAMING_GROUP_ID = 6;
            var updateGameDefinitionMessage = new UpdateGameDefinitionMessage
            {
                GameDefinitionName = "some game definition name",
                Active = true,
                BoardGameGeekGameDefinitionId = 2
            };

            autoMocker.ClassUnderTest.UpdateGameDefinition(updateGameDefinitionMessage, GAME_DEFINITION_ID, GAMING_GROUP_ID);

            autoMocker.Get<IGameDefinitionSaver>().AssertWasCalled(mock => mock.UpdateGameDefinition(Arg<GameDefinitionUpdateRequest>.Matches(
                updateRequest => updateRequest.GameDefinitionId == GAME_DEFINITION_ID
                    && updateRequest.Active == updateGameDefinitionMessage.Active
                    && updateRequest.Name == updateGameDefinitionMessage.GameDefinitionName
                    && updateRequest.BoardGameGeekGameDefinitionId == updateGameDefinitionMessage.BoardGameGeekGameDefinitionId),
                    Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public void ItReturnsANoContentResponse()
        {
            var actualResults = autoMocker.ClassUnderTest.UpdateGameDefinition(new UpdateGameDefinitionMessage(), 0, 0);

            AssertThatApiAction.ReturnsANoContentResponseWithNoContent(actualResults);
        }
    }
}
