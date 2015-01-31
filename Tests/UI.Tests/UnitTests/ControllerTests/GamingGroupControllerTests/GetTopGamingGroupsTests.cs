using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
	[TestFixture]
	public class GetTopGamingGroupsTests : GamingGroupControllerTestBase
	{
		[Test]
		public void ItReturnsTopGamingGroupsView()
		{
			var viewResult = gamingGroupControllerPartialMock.GetTopGamingGroups() as ViewResult;

			Assert.AreEqual(MVC.GamingGroup.Views.TopGamingGroups, viewResult.ViewName);
		}
	}
}
