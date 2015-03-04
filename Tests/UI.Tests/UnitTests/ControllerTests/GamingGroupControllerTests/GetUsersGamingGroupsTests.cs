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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.Owin.Security.Provider;
using NUnit.Framework;
using Rhino.Mocks;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetUsersGamingGroupsTests : GamingGroupControllerTestBase
    {
        private List<GamingGroup> expectedGamingGroups;
            
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            expectedGamingGroups = new List<GamingGroup>
            {
                new GamingGroup
                {
                    Name = "gaming group 1",
                    Id = 1351
                },
                            new GamingGroup
                {
                    Name = "gaming group 1",
                    Id = currentUser.CurrentGamingGroupId.Value
                }
            };
            gamingGroupRetrieverMock.Expect(mock => mock.GetGamingGroupsForUser(Arg<ApplicationUser>.Is.Anything))
                .Return(expectedGamingGroups);
        }

        [Test]
        public void ItReturnsAllGamingGroupsForTheUser()
        {
            JsonResult jsonResult = gamingGroupControllerPartialMock.GetUsersGamingGroups(currentUser) as JsonResult;
            dynamic jsonData = jsonResult.Data;

            Assert.That(jsonData[0].Id, Is.EqualTo(expectedGamingGroups[0].Id));
            Assert.That(jsonData[0].Name, Is.EqualTo(expectedGamingGroups[0].Name));
            Assert.That(jsonData[0].IsCurrentGamingGroup, Is.EqualTo(false));

            Assert.That(jsonData[1].Id, Is.EqualTo(expectedGamingGroups[1].Id));
            Assert.That(jsonData[1].Name, Is.EqualTo(expectedGamingGroups[1].Name));
            Assert.That(jsonData[1].IsCurrentGamingGroup, Is.EqualTo(true));
        }
    }
}
