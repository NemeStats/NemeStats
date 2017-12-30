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

using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class CreateNewGamingGroupTests : GamingGroupControllerTestBase
    {
        [Test]
        public void It_Redirects_Back_To_The_Manage_Account_Page_If_The_Gaming_Group_Name_Isnt_Set()
        {
            var expectedResult = new RedirectResult("some url");
            autoMocker.ClassUnderTest.Expect(mock => mock.MakeRedirectResultToManageAccountPageGamingGroupsTab()).Return(expectedResult);

            var result = autoMocker.ClassUnderTest.CreateNewGamingGroup(string.Empty, currentUser) as RedirectResult;

            result.ShouldNotBeNull();
            result.ShouldBe(expectedResult);
        }

        [Test]
        public void ItSavesTheGamingGroupWithTheRegistrationSourceAsTheWebApplication()
        {
            string gamingGroupName = "name";

            autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, currentUser);

            autoMocker.Get<IGamingGroupSaver>().AssertWasCalled(mock => mock.CreateNewGamingGroup(gamingGroupName, TransactionSource.WebApplication, currentUser));
        }

        [Test]
        public void ItRedirectsToTheDetailsActionAfterSaving()
        {
            string gamingGroupName = "name";

            var result = autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, currentUser) as RedirectToRouteResult;

            Assert.That(result.RouteValues["action"], Is.EqualTo(MVC.GamingGroup.ActionNames.Details));
            Assert.IsNotNull(result.RouteValues["id"]);
        }
    }
}
