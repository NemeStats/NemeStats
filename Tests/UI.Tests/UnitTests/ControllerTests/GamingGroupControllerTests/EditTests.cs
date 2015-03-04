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
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Controllers.Helpers;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class EditTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItUpdatesTheGamingGroupName()
        {
            string gamingGroupName = "new gaming group name";

            HttpStatusCodeResult httpStatusCodeResult = gamingGroupControllerPartialMock.Edit(gamingGroupName, currentUser) as HttpStatusCodeResult;

            gamingGroupSaverMock.AssertWasCalled(saver => saver.UpdateGamingGroupName(gamingGroupName, currentUser));
        }

        [Test]
        public void ItReturnsAJsonResultWhenEverythingIsOk()
        {
            JsonResult jsonResult = gamingGroupControllerPartialMock.Edit("gaming group name", currentUser) as JsonResult;

            dynamic jsonData = jsonResult.Data;
            Assert.AreEqual((int)HttpStatusCode.OK, jsonData.StatusCode);
        }

        [Test]
        public void ItClearsTheGamingGroupCookieAfterRenaming()
        {
            gamingGroupControllerPartialMock.Edit("gaming group name", currentUser);

            cookieHelperMock.AssertWasCalled(mock => mock.ClearCookie(
                 Arg<NemeStatsCookieEnum>.Is.Equal(NemeStatsCookieEnum.gamingGroupsCookie),
                 Arg<HttpRequestBase>.Is.Anything,
                 Arg<HttpResponseBase>.Is.Anything));
        }
    }
}
