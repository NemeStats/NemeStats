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
using UI.Models.User;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class UserGamingGroupsTests : AccountControllerTestBase
    {
        private List<GamingGroupListItemModel> _expectedGamingGroups;
        ViewResult _getResult;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

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
                    Id = currentUser.CurrentGamingGroupId
                }
            };

            gamingGroupRetrieverMock.Stub(s => s.GetGamingGroupsForUser(Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedGamingGroups);


            _getResult = accountControllerPartialMock.UserGamingGroups(currentUser) as ViewResult;
        }

        [Test]
        public void ItReturns_UserGamingGroupView()
        {
            Assert.That(_getResult.ViewName, Is.EqualTo(MVC.Account.Views.UserGamingGroups));
        }

        [Test]
        public void It_Calls_GamingGroupRetriever_GetGamingGroupsForUser()
        {
            gamingGroupRetrieverMock.AssertWasCalled(s=>s.GetGamingGroupsForUser(currentUser));
        }

        [Test]
        public void ItReturns_UserGamingGroupsModel_Filled()
        {
            var model = _getResult.Model as UserGamingGroupsModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(_expectedGamingGroups.Count, model.GamingGroups.Count);
            Assert.AreEqual(currentUser.Id, model.CurrentUser.Id);
            Assert.AreEqual(currentUser.CurrentGamingGroupId, model.CurrentGamingGroup.Id);
        }
    }
}
