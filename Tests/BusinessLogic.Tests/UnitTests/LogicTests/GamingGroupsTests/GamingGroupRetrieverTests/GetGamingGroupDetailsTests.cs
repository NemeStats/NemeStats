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

using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetGamingGroupDetailsTests : GamingGroupRetrieverTestBase
    {
        private GamingGroup _expectedGamingGroup;
        private List<GameDefinitionSummary> _gameDefinitionSummaries;
        private GamingGroupFilter _filter;

        private int _gamingGroupId = 13511;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _expectedGamingGroup = new GamingGroup
            {
                Id = _gamingGroupId, 
                OwningUserId = CurrentUser.Id
            };

            _filter = new GamingGroupFilter
            {
                GamingGroupId = _gamingGroupId
            };

            AutoMocker.Get<IDataContext>().Expect(mock => mock.FindById<GamingGroup>(_gamingGroupId))
                .Return(_expectedGamingGroup);

            _gameDefinitionSummaries = new List<GameDefinitionSummary>
            {
                new GameDefinitionSummary()
            };

            AutoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitions(_gamingGroupId, _filter.DateRangeFilter))
                                       .Return(_gameDefinitionSummaries);

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            applicationUsers.Add(CurrentUser);

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>())
                .Return(applicationUsers.AsQueryable());

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>())
                .Return(applicationUsers.AsQueryable());
        }

        [Test]
        public void ItReturnsTheGamingGroupSummary()
        {
            GamingGroupSummary actualGamingGroup = AutoMocker.ClassUnderTest.GetGamingGroupDetails(_filter);

            Assert.AreEqual(_expectedGamingGroup.Id, actualGamingGroup.Id);
            Assert.AreEqual(_expectedGamingGroup.Name, actualGamingGroup.Name);
            Assert.AreEqual(_expectedGamingGroup.DateCreated, actualGamingGroup.DateCreated);
        }
    }
}
