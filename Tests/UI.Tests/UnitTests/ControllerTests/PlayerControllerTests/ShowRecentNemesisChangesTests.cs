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
using System.Linq;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
	[TestFixture]
	public class ShowRecentNemesisChangesTests : PlayerControllerTestBase
	{
		[Test]
		public void ItReturnsRecentNemesisChangesView()
		{
			var viewResult = autoMocker.ClassUnderTest.ShowRecentNemesisChanges() as ViewResult;

			Assert.AreEqual(MVC.Player.Views.RecentNemesisChanges, viewResult.ViewName);
		}

		[Test]
		public void ItReturnsSpecifiedRecentNemesisChangeModelToView()
		{
			var viewResult = autoMocker.ClassUnderTest.ShowRecentNemesisChanges() as ViewResult;

			var actualViewModel = viewResult.ViewData.Model;

			Assert.AreEqual(base.expectedNemesisChangeViewModel, actualViewModel);
		}
	}
}
