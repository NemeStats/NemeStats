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

using System.Collections.Generic;
using System.Linq;
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
            var actualGamingGroups = gamingGroupRetriever.GetGamingGroupsForUser(currentUser);

            Assert.That(actualGamingGroups.Count, Is.EqualTo(1));
            Assert.That(actualGamingGroups[0].Id, Is.EqualTo(gamingGroupId1));
        }
    }
}
