using System.Linq;
using System.Net;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using BusinessLogic.Models.Games;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.GameDefinitionsControllerTests
{
    [TestFixture]
    public class SaveNewGameDefinitionTests : ApiControllerTestBase<GameDefinitionsController>
    {
        private GameDefinitionDisplayInfo expectedGameDefinitionDisplayInfo;

        [SetUp]
        public void SetUp()
        {
            expectedGameDefinitionDisplayInfo = new GameDefinitionDisplayInfo
            {
                Id = 100
            };

            autoMocker.Get<IGameDefinitionSaver>().Expect(mock => mock.CreateGameDefinition(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                      .Return(expectedGameDefinitionDisplayInfo);
        }

        [Test]
        public void ItSavesTheNewGameDefinition()
        {
            var newGameDefinitionMessage = new NewGameDefinitionMessage
            {
                GameDefinitionName = "some gameDefinitionName",
                BoardGameGeekGameDefinitionId = 1
            };

            autoMocker.ClassUnderTest.SaveNewGameDefinition(newGameDefinitionMessage, 0);

            autoMocker.Get<IGameDefinitionSaver>().AssertWasCalled(
                mock => mock.CreateGameDefinition(Arg<GameDefinition>.Matches(gameDefinition => gameDefinition.Name == newGameDefinitionMessage.GameDefinitionName
                    && gameDefinition.BoardGameGeekGameDefinitionId == newGameDefinitionMessage.BoardGameGeekGameDefinitionId),
                    Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public void ItReturnsANewlyCreatedGameDefinitionMessage()
        {
            var actualResults = autoMocker.ClassUnderTest.SaveNewGameDefinition(new NewGameDefinitionMessage(), 0);

            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<NewlyCreatedGameDefinitionMessage>(actualResults, HttpStatusCode.OK);
            Assert.That(actualData.GameDefinitionId, Is.EqualTo(expectedGameDefinitionDisplayInfo.Id));
        }
    }
}
