using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic.UniversalGameDefinitions;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;
using UI.Controllers;
using UI.Models.UniversalGameModels;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.UniversalGameControllerTests
{
    [TestFixture]
    public class DetailsTests
    {
        private RhinoAutoMocker<UniversalGameController> _autoMocker;

        private int _boardGameGeekGameDefinitionId = 1;
        private ApplicationUser _currentUser;
        private UniversalGameData _expectedUniversalGameData;
        private UniversalGameViewModel _expectedUniversalGameViewModel;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalGameController>();
            _currentUser = new ApplicationUser();

            _expectedUniversalGameData = new UniversalGameData();
            _expectedUniversalGameViewModel = new UniversalGameViewModel();

            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<UniversalGameViewModel>(_expectedUniversalGameData))
                .Return(_expectedUniversalGameViewModel);
        }

        [Test]
        public void It_Returns_The_UniversalGameViewModel()
        {
            //--arrange
            _autoMocker.Get<IUniversalGameRetriever>().Expect(mock => mock.GetResults(_boardGameGeekGameDefinitionId))
                .Return(_expectedUniversalGameData);

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser) as ViewResult;

            //--assert
            result.ShouldNotBeNull();
            result.ViewName.ShouldBe(MVC.UniversalGame.Views.Details);
            result.Model.ShouldBeSameAs(_expectedUniversalGameViewModel);
        }
    }
}
