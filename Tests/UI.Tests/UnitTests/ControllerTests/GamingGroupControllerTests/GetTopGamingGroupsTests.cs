using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			gamingGroupRetrieverMock.Expect(mock => mock.GetTopGamingGroups(Arg<int>.Is.Anything)).Return(new List<TopGamingGroupSummary>());
			gamingGroupControllerPartialMock.Expect(mock => mock.GetTopGamingGroups()).Return(new ViewResult() { ViewData = new ViewDataDictionary(expectedViewModel) });
		}

		[Test]
		public void ItReturnsTopGamingGroupsView()
		{
			var viewResult = gamingGroupControllerPartialMock.GetTopGamingGroups() as ViewResult;

			Assert.AreEqual(MVC.GamingGroup.Views.TopGamingGroups, viewResult.ViewName);
		}

		[Test]
		public void ItReturnsSpecifiedTopGamingGroupsModelToTheView()
		{
			var viewResult = gamingGroupControllerPartialMock.GetTopGamingGroups() as ViewResult;

			var actualViewModel = viewResult.ViewData.Model;

			Assert.AreEqual(expectedViewModel, actualViewModel);
		}
	}
}
