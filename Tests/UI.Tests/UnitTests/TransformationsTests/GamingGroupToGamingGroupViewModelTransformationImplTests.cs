using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
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
    public class GamingGroupToGamingGroupViewModelTransformationImplTests
    {
        private GamingGroupToGamingGroupViewModelTransformationImpl transformer;
        private GamingGroupInvitationToInvitationViewModelTransformation invitationTransformer;
        private GamingGroup gamingGroup;
        private GamingGroupViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            invitationTransformer = MockRepository.GenerateMock<GamingGroupInvitationToInvitationViewModelTransformation>();
            transformer = new GamingGroupToGamingGroupViewModelTransformationImpl(invitationTransformer);
            ApplicationUser owningUser = new ApplicationUser()
            {
                Id = "owning user user Id",
                Email = "owninguser@email.com",
                UserName = "username"
            };
            ApplicationUser registeredUser = new ApplicationUser()
            {
                Email = "registereduser@email.com",
                Id = "registered user id",
                UserName = "registered user name"
            };
            GamingGroupInvitation invitation = new GamingGroupInvitation()
            {
                DateRegistered = DateTime.UtcNow,
                RegisteredUserId = "registered user id",
                RegisteredUser = registeredUser
            };
            gamingGroup = new GamingGroup()
            {
                Id = 1,
                Name = "gaming group",
                OwningUserId = owningUser.Id,
                OwningUser = owningUser,
                GamingGroupInvitations = new List<GamingGroupInvitation>() { invitation }
            };

            viewModel = transformer.Build(gamingGroup);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(gamingGroup.Id, viewModel.Id);
        }

        [Test]
        public void ItCopiesTheOwningUserId()
        {
            Assert.AreEqual(gamingGroup.OwningUserId, viewModel.OwningUserId);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(gamingGroup.Name, viewModel.Name);
        }
        
        [Test]
        public void ItCopiesTheOwningUserName()
        {
            Assert.AreEqual(gamingGroup.OwningUser.UserName, viewModel.OwningUserName);
        }

        [Test]
        public void ItTransformsGamingGroupInvitationsToInvitationViewModels()
        {
            List<InvitationViewModel> invitations = new List<InvitationViewModel>();
            foreach(GamingGroupInvitation invitation in gamingGroup.GamingGroupInvitations)
            {
                InvitationViewModel invitationViewModel = new InvitationViewModel();
                invitations.Add(invitationViewModel);
                
                invitationTransformer.Expect(mock => mock.Build(invitation))
                    .Repeat.Once()
                    .Return(invitationViewModel);
            }

            Assert.AreEqual(invitations.Count(), viewModel.Invitations.Count());
        }
    }
}
