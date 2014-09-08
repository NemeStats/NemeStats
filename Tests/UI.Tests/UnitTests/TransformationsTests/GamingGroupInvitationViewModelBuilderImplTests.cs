using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.GamingGroup;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GamingGroupInvitationViewModelBuilderImplTests
    {
        protected GamingGroupInvitationViewModelBuilderImpl transformation;
        protected GamingGroupInvitation gamingGroupInvitation;
        protected InvitationViewModel invitationViewModel;

        [SetUp]
        public void SetUp()
        {
            transformation = new GamingGroupInvitationViewModelBuilderImpl();
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
