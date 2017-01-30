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
        private GameDefinition _expectedGameDefinition;
        private readonly int _expectedGamingGroupId = 1;

        [SetUp]
        public void SetUp()
        {
            _expectedGameDefinition = new GameDefinition
            {
                Id = 100,
                GamingGroupId = _expectedGamingGroupId
            };

            _autoMocker.Get<ICreateGameDefinitionComponent>().Expect(mock => mock.Execute(Arg<CreateGameDefinitionRequest>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                      .Return(_expectedGameDefinition);
        }

        [Test]
        public void ItSavesTheNewGameDefinition()
        {
            var newGameDefinitionMessage = new NewGameDefinitionMessage
            {
                GameDefinitionName = "some gameDefinitionName",
                BoardGameGeekObjectId = 1,
            };

            _autoMocker.ClassUnderTest.SaveNewGameDefinition(newGameDefinitionMessage, _expectedGamingGroupId);

            _autoMocker.Get<ICreateGameDefinitionComponent>().AssertWasCalled(
                mock => mock.Execute(Arg<CreateGameDefinitionRequest>.Matches(
                    gameDefinition => gameDefinition.Name == newGameDefinitionMessage.GameDefinitionName
                    && gameDefinition.BoardGameGeekGameDefinitionId == newGameDefinitionMessage.BoardGameGeekObjectId
                    && gameDefinition.GamingGroupId == _expectedGamingGroupId),
                    Arg<ApplicationUser>.Is.Same(_applicationUser)));
        }

        [Test]
        public void ItReturnsANewlyCreatedGameDefinitionMessage()
        {
            var actualResults = _autoMocker.ClassUnderTest.SaveNewGameDefinition(new NewGameDefinitionMessage(), _expectedGamingGroupId);

            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<NewlyCreatedGameDefinitionMessage>(actualResults, HttpStatusCode.OK);
            Assert.That(actualData.GameDefinitionId, Is.EqualTo(_expectedGameDefinition.Id));
            Assert.That(actualData.GamingGroupId, Is.EqualTo(_expectedGamingGroupId));
        }

        [Test]
        public void ItSavesTheNewGameDefinitionUsingTheGamingGroupIdOnTheRequestIfOneIsSpecified()
        {
            var newGameDefinitionMessage = new NewGameDefinitionMessage
            {
                GameDefinitionName = "some gameDefinitionName",
                BoardGameGeekObjectId = 1,
                GamingGroupId = _expectedGamingGroupId
            };
            int someGamingGroupIdThatWontGetUsed = -100;

            _autoMocker.ClassUnderTest.SaveNewGameDefinition(newGameDefinitionMessage, someGamingGroupIdThatWontGetUsed);

            _autoMocker.Get<ICreateGameDefinitionComponent>().AssertWasCalled(
                mock => mock.Execute(Arg<CreateGameDefinitionRequest>.Matches(
                    gameDefinition => gameDefinition.Name == newGameDefinitionMessage.GameDefinitionName
                    && gameDefinition.BoardGameGeekGameDefinitionId == newGameDefinitionMessage.BoardGameGeekObjectId
                    && gameDefinition.GamingGroupId == newGameDefinitionMessage.GamingGroupId),
                    Arg<ApplicationUser>.Is.Same(_applicationUser)));
        }
    }
}
