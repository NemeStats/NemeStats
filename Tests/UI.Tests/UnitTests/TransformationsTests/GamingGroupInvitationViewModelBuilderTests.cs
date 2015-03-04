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
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Linq;
using UI.Models.GamingGroup;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GamingGroupInvitationViewModelBuilderTests
    {
        protected GamingGroupInvitationViewModelBuilder transformation;
        protected GamingGroupInvitation gamingGroupInvitation;
        protected InvitationViewModel invitationViewModel;

        [SetUp]
        public void SetUp()
        {
            transformation = new GamingGroupInvitationViewModelBuilder();
            gamingGroupInvitation = new GamingGroupInvitation()
            {
                InviteeEmail = "email@email.com",
                DateRegistered = DateTime.UtcNow,
                RegisteredUser = new ApplicationUser() { UserName = "registered user name" }
            };

            invitationViewModel = transformation.Build(gamingGroupInvitation);
        }

        [Test]
        public void ItCopiesTheInviteeEmail()
        {
            Assert.AreEqual(gamingGroupInvitation.InviteeEmail, invitationViewModel.InviteeEmail);
        }

        [Test]
        public void ItCopiesTheDateRegistered()
        {
            Assert.AreEqual(gamingGroupInvitation.DateRegistered, invitationViewModel.DateRegistered);
        }

        [Test]
        public void ItCopiesTheRegisteredUserName()
        {
            Assert.AreEqual(gamingGroupInvitation.RegisteredUser.UserName, invitationViewModel.RegisteredUserName);
        }

        [Test]
        public void ItSetsTheRegisteredUserNameToBlankIfThereIsNoRegisteredUser()
        {
            gamingGroupInvitation.RegisteredUser = null;
            invitationViewModel = transformation.Build(gamingGroupInvitation);
            Assert.AreEqual(string.Empty, invitationViewModel.RegisteredUserName);
        }
    }
}
