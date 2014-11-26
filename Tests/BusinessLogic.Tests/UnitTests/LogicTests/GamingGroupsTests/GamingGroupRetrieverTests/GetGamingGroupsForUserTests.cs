using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    public class GetGamingGroupsForUserTests : GamingGroupRetrieverTestBase
    {
        private List<GamingGroup> gamingGroupList;
        private int gamingGroupId1 = 1;
        private int gamingGroupId2 = 2;

        public override void SetUp()
        {
            base.SetUp();

            gamingGroupList = new List<GamingGroup>
            {
                new GamingGroup
                {
                    Id = gamingGroupId1,
                    UserGamingGroups = new List<UserGamingGroup>
                    {
                        new UserGamingGroup
                        {
                            ApplicationUserId = currentUser.Id
                        }
                    }
                },
                new GamingGroup
                {
                    Id = gamingGroupId2,
                    UserGamingGroups = new List<UserGamingGroup>
                    {
                        new UserGamingGroup
                        {
                            ApplicationUserId = "some other application id"
                        }
                    }
                }
            };

            dataContextMock.Expect(mock => mock.GetQueryable<GamingGroup>())
                           .Return(gamingGroupList.AsQueryable());
        }

        [Test]
        public void ItRetrievesOnlyGamingGroupsThatTheUserHasAccessTo()
        {
            List<GamingGroup> actualGamingGroups = gamingGroupRetriever.GetGamingGroupsForUser(currentUser);

            Assert.That(actualGamingGroups.Count, Is.EqualTo(1));
            Assert.That(actualGamingGroups[0].Id, Is.EqualTo(gamingGroupId1));
        }
    }
}
