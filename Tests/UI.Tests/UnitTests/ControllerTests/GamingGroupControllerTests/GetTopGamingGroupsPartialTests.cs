#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using NUnit.Framework;
using System.Web.Mvc;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetTopGamingGroupsPartialTests : GamingGroupControllerTestBase
    {
        [Test]
        public void It_Returns_The_TopGamingGroups_Partial_View_With_The_Default_Number_Of_Gaming_Groups()
        {
            //--arrange
            var expectedViewModel = new GamingGroupsSummaryViewModel();
            autoMocker.ClassUnderTest.Expect(partialMock => partialMock.GetGamingGroupsSummaryViewModel(Arg<int>.Is.Anything))
                .Return(expectedViewModel);

            //--act
            var results = autoMocker.ClassUnderTest.GetTopGamingGroupsPartial();

            //--assert
            autoMocker.ClassUnderTest.AssertWasCalled(partialMock => partialMock.GetGamingGroupsSummaryViewModel(Arg<int>.Is.Equal(GamingGroupController.NUMBER_OF_TOP_GAMING_GROUPS_TO_SHOW_ON_HOME_PAGE)));
            var viewResult = results as PartialViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.GamingGroup.Views._TopGamingGroupsPartial);
            var viewModel = viewResult.Model as GamingGroupsSummaryViewModel;
            viewModel.ShouldBeSameAs(expectedViewModel);
        }

        [Test]
        public void It_Returns_The_Specified_Number_Of_Gaming_Groups()
        {
            //--arrange
            var expectedViewModel = new GamingGroupsSummaryViewModel();
            autoMocker.ClassUnderTest.Expect(partialMock => partialMock.GetGamingGroupsSummaryViewModel(Arg<int>.Is.Anything))
                .Return(expectedViewModel);
            int numberOfGamingGroups = 135;

            //--act
            autoMocker.ClassUnderTest.GetTopGamingGroupsPartial(numberOfGamingGroups);

            //--assert
            autoMocker.ClassUnderTest.AssertWasCalled(partialMock => partialMock.GetGamingGroupsSummaryViewModel(Arg<int>.Is.Equal(numberOfGamingGroups)));
        }
    }
}
