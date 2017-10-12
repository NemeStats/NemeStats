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
using BusinessLogic.Models.User;
using Shouldly;
using UI.Models.Home;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class IndexTests : HomeControllerTestBase
    {
        [Test]
        public void It_Returns_An_Index_View_With_Show_Quick_Stats_Set_To_False_For_Anonymous_Users()
        {
            var viewResult = _autoMocker.ClassUnderTest.Index(new AnonymousApplicationUser()) as ViewResult;

            viewResult.ViewName.ShouldBe(MVC.Home.Views.Index);
            var model = viewResult.Model as HomeIndexViewModel;
            model.ShouldNotBeNull();
            model.ShowQuickStats.ShouldBe(false);
        }

        [Test]
        public void It_Shows_The_The_Player_Quick_Stats_If_The_User_Has_A_Current_Gaming_Group()
        {
            var user = new ApplicationUser
            {
                CurrentGamingGroupId = 1
            };
            var viewResult = _autoMocker.ClassUnderTest.Index(user) as ViewResult;

            var model = viewResult.Model as HomeIndexViewModel;
            model.ShowQuickStats.ShouldBe(true);
        }
    }
}
