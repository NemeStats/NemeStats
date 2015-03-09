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
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupSaverTests
{
    [TestFixture]
    public class UpdateGamingGroupNameTests : GamingGroupSaverTestBase
    {
        private readonly GamingGroup expectedGamingGroup = new GamingGroup();
        private readonly GamingGroup savedGamingGroup = new GamingGroup();

        [Test]
        public override void SetUp()
        {
            base.SetUp();

            dataContextMock.Expect(mock => mock.FindById<GamingGroup>(currentUser.CurrentGamingGroupId.Value))
                .Return(expectedGamingGroup);
            dataContextMock.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(savedGamingGroup);
        }

        [Test]
        public void ItSetsTheGamingGroupName()
        {
            gamingGroupSaver.UpdateGamingGroupName(gamingGroupName, currentUser);

            dataContextMock.AssertWasCalled(dataContext => dataContext.Save(
                Arg<GamingGroup>.Matches(gamingGroup => gamingGroup.Name == gamingGroupName),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItReturnsTheGamingGroupThatWasSaved()
        {
            GamingGroup gamingGroup = gamingGroupSaver.UpdateGamingGroupName(gamingGroupName, currentUser);

            Assert.AreSame(savedGamingGroup, gamingGroup);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGamingGroupNameIsNull()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => gamingGroupSaver.UpdateGamingGroupName(null, currentUser));

            Assert.That(exception.Message, Is.EqualTo(GamingGroupSaver.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK));
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGamingGroupNameIsWhitespace()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => gamingGroupSaver.UpdateGamingGroupName("      ", currentUser));

            Assert.That(exception.Message, Is.EqualTo(GamingGroupSaver.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK));
        }
    }
}
