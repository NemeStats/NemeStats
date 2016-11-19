using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
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
        private BoardGameGeekGameSummary _expectedBoardGameGeekGameSummary;
        private UniversalGameDetailsViewModel _expectedUniversalGameDetailsViewModel;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalGameController>();
            _currentUser = new ApplicationUser();

            _expectedBoardGameGeekGameSummary = new BoardGameGeekGameSummary();
            _expectedUniversalGameDetailsViewModel = new UniversalGameDetailsViewModel();

            _autoMocker.Get<IUniversalGameRetriever>().Expect(mock => mock.GetBoardGameGeekGameSummary(_boardGameGeekGameDefinitionId, _currentUser))
                .Return(_expectedBoardGameGeekGameSummary);
            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<UniversalGameDetailsViewModel>(_expectedBoardGameGeekGameSummary))
                .Return(_expectedUniversalGameDetailsViewModel);
        }

        [Test]
        public void It_Returns_The_UniversalGameDetailsViewModel()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser) as ViewResult;

            //--assert
            result.ShouldNotBeNull();
            result.ViewName.ShouldBe(MVC.UniversalGame.Views.Details);
            result.Model.ShouldBeSameAs(_expectedUniversalGameDetailsViewModel);
        }
    }
}
