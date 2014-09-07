using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.GameDefinitionModels;
using UI.Models.GamingGroup;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.Player;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GamingGroupToGamingGroupViewModelTransformationImplTests
    {
        private GamingGroupToGamingGroupViewModelTransformationImpl transformer;
        private GamingGroupInvitationToInvitationViewModelTransformation invitationTransformerMock;
        private PlayerDetailsViewModelBuilder playerDetailsViewModelBuilderMock;
        private GamingGroup gamingGroup;
        private GamingGroupViewModel viewModel;
        private List<Player> players;
        private List<GameDefinition> gameDefinitions;

        [SetUp]
        public void SetUp()
        {
            invitationTransformerMock = MockRepository.GenerateMock<GamingGroupInvitationToInvitationViewModelTransformation>();
            playerDetailsViewModelBuilderMock = MockRepository.GenerateMock<PlayerDetailsViewModelBuilder>();
            transformer = new GamingGroupToGamingGroupViewModelTransformationImpl(invitationTransformerMock, playerDetailsViewModelBuilderMock);
            players = new List<Player>();
            gameDefinitions = new List<GameDefinition>();
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
                GamingGroupInvitations = new List<GamingGroupInvitation>() { invitation },
                Players = players,
                GameDefinitions = gameDefinitions
            };

            viewModel = transformer.Build(gamingGroup, null);
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

                invitationTransformerMock.Expect(mock => mock.Build(invitation))
                    .Repeat.Once()
                    .Return(invitationViewModel);
            }

            Assert.AreEqual(invitations.Count(), viewModel.Invitations.Count());
        }

        [Test]
        public void ItSetsThePlayers()
        {
            Assert.AreSame(players, viewModel.Players);
        }

        [Test]
        public void ItSetsTheGameDefinitions()
        {
            Assert.AreSame(gameDefinitions, viewModel.GameDefinitions);
        }
    }
}
