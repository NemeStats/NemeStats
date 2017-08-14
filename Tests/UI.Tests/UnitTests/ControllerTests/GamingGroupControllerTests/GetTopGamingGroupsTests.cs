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
using Rhino.Mocks;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
	[TestFixture]
	public class GetTopGamingGroupsTests : GamingGroupControllerTestBase
	{
		private readonly TopGamingGroupSummaryViewModel expectedViewModel = new TopGamingGroupSummaryViewModel();

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
            autoMocker.PartialMockTheClassUnderTest();
			autoMocker.Get<IGamingGroupRetriever>().Expect(mock => mock.GetTopGamingGroups(Arg<int>.Is.Anything)).Return(new List<TopGamingGroupSummary>());
			autoMocker.ClassUnderTest.Expect(mock => mock.GetTopGamingGroups()).Return(new ViewResult() { ViewName = MVC.GamingGroup.Views.TopGamingGroups, ViewData = new ViewDataDictionary(expectedViewModel) });
		}

		[Test]
		public void ItReturnsTopGamingGroupsView()
		{
			var viewResult = autoMocker.ClassUnderTest.GetTopGamingGroups() as ViewResult;

			Assert.AreEqual(MVC.GamingGroup.Views.TopGamingGroups, viewResult.ViewName);
		}

		[Test]
		public void ItReturnsSpecifiedTopGamingGroupsModelToTheView()
		{
			var viewResult = autoMocker.ClassUnderTest.GetTopGamingGroups() as ViewResult;

			var actualViewModel = viewResult.ViewData.Model;

			Assert.AreEqual(expectedViewModel, actualViewModel);
		}
	}
}
