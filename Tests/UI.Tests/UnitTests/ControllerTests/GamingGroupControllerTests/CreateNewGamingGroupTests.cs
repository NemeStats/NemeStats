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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers.Helpers;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class CreateNewGamingGroupTests : GamingGroupControllerTestBase
    {
        private readonly ViewResult viewResult = new ViewResult();

        [SetUp]
        public override void SetUp()
        {
 	        base.SetUp();
            gamingGroupControllerPartialMock.Expect(mock => mock.Index(currentUser))
                                .Return(viewResult);
        }

        [Test]
        public void ItRemainsOnTheIndexPageIfTheGamingGroupNameIsntSet()
        {
            var result = gamingGroupControllerPartialMock.CreateNewGamingGroup(string.Empty, currentUser) as ViewResult;

            Assert.That(result, Is.SameAs(viewResult));
        }

        [Test]
        public void ItSavesTheGamingGroup()
        {
            string gamingGroupName = "name";

            gamingGroupControllerPartialMock.CreateNewGamingGroup(gamingGroupName, currentUser);

            gamingGroupSaverMock.AssertWasCalled(mock => mock.CreateNewGamingGroup(gamingGroupName, currentUser));
        }

        [Test]
        public void ItRedirectsToTheIndexActionAfterSaving()
        {
            string gamingGroupName = "name";

            var result = gamingGroupControllerPartialMock.CreateNewGamingGroup(gamingGroupName, currentUser) as RedirectToRouteResult;

            Assert.That(result.RouteValues["action"], Is.EqualTo(MVC.GamingGroup.ActionNames.Index));
            Assert.That(result.RouteValues["controller"], Is.EqualTo(MVC.GamingGroup.Name));

        }

        [Test]
        public async Task ItClearsTheGamingGroupCookieIfTheUserSuccessfullyRegisters()
        {
            var result = gamingGroupControllerPartialMock.CreateNewGamingGroup("some name", currentUser) as RedirectToRouteResult;

            cookieHelperMock.AssertWasCalled(mock => mock.ClearCookie(
                Arg<NemeStatsCookieEnum>.Is.Equal(NemeStatsCookieEnum.gamingGroupsCookie),
                Arg<HttpRequestBase>.Is.Anything,
                Arg<HttpResponseBase>.Is.Anything));
        }
    }
}
