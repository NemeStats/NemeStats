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
using System.Web.Mvc;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.User;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class UserGamingGroupsTests : AccountControllerTestBase
    {
        private List<GamingGroupListItemModel> _expectedGamingGroups;

        private void SetupTwoActiveGamingGroups()
        {
            _expectedGamingGroups = new List<GamingGroupListItemModel>
            {
                new GamingGroupListItemModel
                {
                    Name = "gaming group 1",
                    Id = 1351
                },
                new GamingGroupListItemModel
                {
                    Name = "gaming group 1",
                    Id = currentUser.CurrentGamingGroupId.Value
                }
            };

            gamingGroupRetrieverMock.Stub(s => s.GetGamingGroupsForUser(Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedGamingGroups);
        }

        [Test]
        public void It_Returns_UserGamingGroups_View()
        {
            SetupTwoActiveGamingGroups();

            var result = accountControllerPartialMock.UserGamingGroups(currentUser) as PartialViewResult;

            result.ShouldNotBeNull();
            result.ViewName.ShouldBe(MVC.Account.Views._UserGamingGroupsPartial);
            gamingGroupRetrieverMock.AssertWasCalled(s => s.GetGamingGroupsForUser(currentUser));
            var model = result.Model as UserGamingGroupsModel;
            model.ShouldNotBeNull();
            model.GamingGroups.Count.ShouldBe(_expectedGamingGroups.Count);
            model.CurrentGamingGroup.Id.ShouldBe(currentUser.CurrentGamingGroupId.Value);
            model.CurrentUser.ShouldBe(currentUser);
        }

    }
}
