#region LICENSE

// NemeStats is a free website for tracking the results of board games. Copyright (C) 2015 Jacob Gordon
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this program. If
// not, see <http://www.gnu.org/licenses/>

#endregion LICENSE

using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using UI.Models.GamingGroup;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class DetailsTests : GamingGroupControllerTestBase
    {
        private GamingGroupSummary gamingGroupSummary;
        private GamingGroupViewModel _gamingGroupViewModel;
        private BasicDateRangeFilter dateRangeFilter;

        [Test]
        public override void SetUp()
        {
            base.SetUp();

            gamingGroupSummary = new GamingGroupSummary()
            {
                PlayedGames = new List<PlayedGame>()
            };
            _gamingGroupViewModel = new GamingGroupViewModel();
            dateRangeFilter = new BasicDateRangeFilter();

            autoMocker.ClassUnderTest.Expect(mock => mock.GetGamingGroupSummary(
                Arg<int>.Is.Anything,
                Arg<IDateRangeFilter>.Is.Anything))
                .Repeat.Once()
                .Return(gamingGroupSummary);

            autoMocker.Get<IGamingGroupViewModelBuilder>().Expect(mock => mock.Build(gamingGroupSummary, currentUser))
                .Return(_gamingGroupViewModel);
        }

        [Test]
        public void ItReturnsTheDetailsView()
        {
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId , currentUser, dateRangeFilter) as ViewResult;

            Assert.AreEqual(MVC.GamingGroup.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItAddsAGamingGroupViewModelToTheView()
        {
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId, currentUser, dateRangeFilter) as ViewResult;

            Assert.AreSame(_gamingGroupViewModel, viewResult.Model);
        }

        [Test]
        public void ItPreservesTheDateRangeFilter()
        {
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId, currentUser, dateRangeFilter) as ViewResult;

            var model = viewResult.Model as GamingGroupViewModel;
            Assert.AreSame(dateRangeFilter, model.DateRangeFilter);
        }

        [Test]
        public void ItShowsTheSearchPlayedGamesLinkInThePlayedGamePanelHeader()
        {
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId, currentUser, dateRangeFilter) as ViewResult;

            var viewModel = (GamingGroupViewModel)viewResult.Model;
            Assert.That(viewModel.PlayedGames.ShowSearchLinkInResultsHeader, Is.True);
        }

        [Test]
        public void ItAddsAModelErrorIfTheBasicDateRangeFilterHasValidationErrors()
        {
            var basicDateRangeFilterMock = MockRepository.GenerateMock<BasicDateRangeFilter>();
            var expectedErrorMessage = "some error message";
            basicDateRangeFilterMock.Expect(mock => mock.IsValid(out Arg<string>.Out(expectedErrorMessage).Dummy)).Return(false);
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId, currentUser, basicDateRangeFilterMock) as ViewResult;

            Assert.True(viewResult.ViewData.ModelState.ContainsKey("dateRangeFilter"));
            var modelErrorsForKey = viewResult.ViewData.ModelState["dateRangeFilter"].Errors;
            Assert.That(modelErrorsForKey.Count, Is.EqualTo(1));
            Assert.That(modelErrorsForKey[0].ErrorMessage, Is.EqualTo(expectedErrorMessage));
        }
    }
}