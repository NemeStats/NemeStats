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

using BusinessLogic.Exceptions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using System;
using System.Net;
using System.Web.Mvc;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class DetailsTests : GamingGroupControllerTestBase
    {
        private GamingGroupSummary _gamingGroupSummary;
        private BasicDateRangeFilter _dateRangeFilter;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _gamingGroupSummary = new GamingGroupSummary
            {
                Id = 1,
                Name = "some gaming group name",
                DateCreated = DateTime.MaxValue,
                PublicDescription = "some public description",
                PublicGamingGroupWebsite = "https://website.com",
                Active = true
            };
            _dateRangeFilter = new BasicDateRangeFilter();
        }

        private void SetUpAutoMockerToReturnSummary()
        {
            autoMocker.ClassUnderTest.Expect(mock => mock.GetGamingGroupSummary(
                Arg<int>.Is.Anything,
                Arg<IDateRangeFilter>.Is.Anything))
                .Repeat.Once()
                .Return(_gamingGroupSummary);
        }

        [Test]
        public void ItReturnsTheDetailsView()
        {
            SetUpAutoMockerToReturnSummary();
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId.Value, currentUser, _dateRangeFilter) as ViewResult;

            Assert.AreEqual(MVC.GamingGroup.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItAddsAGamingGroupViewModelToTheView()
        {
            SetUpAutoMockerToReturnSummary();
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId.Value, currentUser, _dateRangeFilter) as ViewResult;

            var viewModel = viewResult.Model as GamingGroupViewModel;
            viewModel.ShouldNotBeNull();
            viewModel.PublicDetailsView.GamingGroupId.ShouldBe(_gamingGroupSummary.Id);
            viewModel.PublicDetailsView.GamingGroupName.ShouldBe(_gamingGroupSummary.Name);
            viewModel.PublicDetailsView.PublicDescription.ShouldBe(_gamingGroupSummary.PublicDescription);
            viewModel.PublicDetailsView.Website.ShouldBe(_gamingGroupSummary.PublicGamingGroupWebsite);
            viewModel.PublicDetailsView.Active.ShouldBe(_gamingGroupSummary.Active);
        }

        [Test]
        public void ItPreservesTheDateRangeFilter()
        {
            SetUpAutoMockerToReturnSummary();
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId.Value, currentUser, _dateRangeFilter) as ViewResult;

            var model = viewResult.Model as GamingGroupViewModel;
            Assert.AreSame(_dateRangeFilter, model.DateRangeFilter);
        }

        [Test]
        public void ItAddsAModelErrorIfTheBasicDateRangeFilterHasValidationErrors()
        {
            SetUpAutoMockerToReturnSummary();
            var basicDateRangeFilterMock = MockRepository.GenerateMock<BasicDateRangeFilter>();
            var expectedErrorMessage = "some error message";
            basicDateRangeFilterMock.Expect(mock => mock.IsValid(out Arg<string>.Out(expectedErrorMessage).Dummy)).Return(false);
            var viewResult = autoMocker.ClassUnderTest.Details(currentUser.CurrentGamingGroupId.Value, currentUser, basicDateRangeFilterMock) as ViewResult;

            Assert.True(viewResult.ViewData.ModelState.ContainsKey("dateRangeFilter"));
            var modelErrorsForKey = viewResult.ViewData.ModelState["dateRangeFilter"].Errors;
            Assert.That(modelErrorsForKey.Count, Is.EqualTo(1));
            Assert.That(modelErrorsForKey[0].ErrorMessage, Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void ItReturnsAnHttpNotFoundStatusCodeIfTheGamingGroupIsNotFound()
        {
            const int nonExistentGamingGroupId = -1;
            autoMocker.Get<IGamingGroupRetriever>().BackToRecord(BackToRecordOptions.All);
            autoMocker.Get<IGamingGroupRetriever>().Expect(mock => mock.GetGamingGroupDetails(Arg<GamingGroupFilter>.Is.Anything))
                .Throw(new EntityDoesNotExistException<GamingGroup>(nonExistentGamingGroupId));
            autoMocker.Get<IGamingGroupRetriever>().Replay();

            var result = autoMocker.ClassUnderTest.Details(nonExistentGamingGroupId, currentUser) as HttpStatusCodeResult;

            Assert.That(result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }
    }
}