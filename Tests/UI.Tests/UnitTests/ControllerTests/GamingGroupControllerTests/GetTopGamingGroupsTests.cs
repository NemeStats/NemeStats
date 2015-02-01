using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Rhino.Mocks;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
	[TestFixture]
	public class GetTopGamingGroupsTests : GamingGroupControllerTestBase
	{
        [SetUp]
	    public override void SetUp()
	    {
	        base.SetUp();

            gamingGroupRetrieverMock.Expect(mock => mock.GetTopGamingGroups(Arg<int>.Is.Anything)).Return(new List<TopGamingGroupSummary>());
	    }

	    [Test]
		public void ItReturnsTopGamingGroupsView()
		{
			var viewResult = gamingGroupControllerPartialMock.GetTopGamingGroups() as ViewResult;

			Assert.AreEqual(MVC.GamingGroup.Views.TopGamingGroups, viewResult.ViewName);
		}
	}
}
