using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;
using UI.Controllers;
using UI.Models.Achievements;

namespace UI.Tests.UnitTests.ControllerTests.AchievementControllerTests
{
    [TestFixture]
    public class IndexTests
    {
        private RhinoAutoMocker<AchievementController> _autoMocker;
        private ApplicationUser _currentUser;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<AchievementController>();
            _currentUser = new ApplicationUser
            {
                Id = "some id"
            };
        }

        [Test]
        public void It_Returns_All_Achievements()
        {
            //--arrange
            var achievementSummaries = new List<AchievementSummary>();
            _autoMocker.Get<IAchievementRetriever>().Expect(mock => mock.GetAllAchievementSummaries(_currentUser)).Return(achievementSummaries);
            var expectedAchievementViewModels = new List<AchievementTileViewModel>();
            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<List<AchievementTileViewModel>>(achievementSummaries))
                .Return(expectedAchievementViewModels);

            //--act
            var results = _autoMocker.ClassUnderTest.Index(_currentUser) as ViewResult;

            //--assert
            results.ShouldNotBeNull();
            var viewModel = results.Model as AchievementListViewModel;
            viewModel.ShouldNotBeNull();
            viewModel.CurrentUserId.ShouldBe(_currentUser.Id);
            viewModel.Achievements.ShouldBeSameAs(expectedAchievementViewModels);
        }

    }
}
