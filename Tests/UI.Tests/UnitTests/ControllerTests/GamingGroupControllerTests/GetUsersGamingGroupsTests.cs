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
