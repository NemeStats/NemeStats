using BusinessLogic.DataAccess;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.ApplicationLinkerTests
{
    [TestFixture]
    public class ApplicationLinkerTests
    {
        private RhinoAutoMocker<ApplicationLinker> _autoMocker;
        private IDataContext _dataContext;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<ApplicationLinker>();
            _dataContext = MockRepository.GenerateMock<IDataContext>();
        }

        [Test]
        public void It_Links_The_Played_Game_To_The_Specified_Application_Using_The_Specified_Entity_Id()
        {
            //--arrange
            int playedGameId = 1;
            string applicationName = "some app name";
            string entityId = "some entity id";

            //--act
            _autoMocker.ClassUnderTest.LinkApplication(playedGameId, applicationName, entityId, _dataContext);

            //--assert
            var callsMadeOn = _dataContext.GetArgumentsForCallsMadeOn(
                mock => mock.Save(Arg<PlayedGameApplicationLinkage>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            callsMadeOn.ShouldNotBeNull();
            callsMadeOn.Count.ShouldBe(1);
            var firstCall = callsMadeOn[0];
            var applicationLinkage = firstCall[0] as PlayedGameApplicationLinkage;
            applicationLinkage.ShouldNotBeNull();
            applicationLinkage.PlayedGameId.ShouldBe(playedGameId);
            applicationLinkage.ApplicationName.ShouldBe(applicationName);
            applicationLinkage.EntityId.ShouldBe(entityId);

            var applicationUser = firstCall[1];
            applicationUser.ShouldBeAssignableTo<AnonymousApplicationUser>();
        }

    }
}
