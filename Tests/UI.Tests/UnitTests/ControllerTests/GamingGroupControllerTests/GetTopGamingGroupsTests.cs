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

using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic;
using Rhino.Mocks;
using Shouldly;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
	[TestFixture]
	public class GetTopGamingGroupsTests : GamingGroupControllerTestBase
	{
		[Test]
		public void It_Returns_The_Specified_Number_Of_Top_Gaming_Groups()
		{
            //--arrange
            var expectedTopGamingGroupSummary = new TopGamingGroupSummary();
		    var gamingGroupList = new List<TopGamingGroupSummary>
		    {
                expectedTopGamingGroupSummary
            };
            autoMocker.Get<IGamingGroupRetriever>().Expect(mock => mock.GetTopGamingGroups(Arg<int>.Is.Anything)).Return(gamingGroupList);

            var expectedViewModel = new TopGamingGroupSummaryViewModel();
		    autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<TopGamingGroupSummaryViewModel>(expectedTopGamingGroupSummary))
		        .Return(expectedViewModel);

            //--act
            var viewResult = autoMocker.ClassUnderTest.GetTopGamingGroups() as PartialViewResult;

            //--assert
            viewResult.ShouldNotBeNull();
		    viewResult.ViewName.ShouldBe(MVC.GamingGroup.Views.TopGamingGroups);
            var actualViewModel = viewResult.ViewData.Model as List<TopGamingGroupSummaryViewModel>;
            actualViewModel.ShouldNotBeNull();
            actualViewModel.Count.ShouldBe(1);
            actualViewModel[0].ShouldBeSameAs(expectedViewModel);
        }
	}
}
