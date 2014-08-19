using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverImplTests
{
    [TestFixture]
    public class GetGamingGroupDetailsTests
    {
        protected GamingGroupRetrieverImpl gamingGroupRetriever;
        protected DataContext dataContextMock;
        protected ApplicationUser currentUser;
        protected GamingGroup expectedGamingGroup;
        protected GamingGroupInvitation expectedGamingGroupInvitation;

        protected int gamingGroupId = 13511;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            gamingGroupRetriever = new GamingGroupRetrieverImpl(dataContextMock);

            currentUser = new ApplicationUser() { Id = "application user", UserName = "user name" };
            expectedGamingGroup = new GamingGroup() { Id = gamingGroupId, OwningUserId = currentUser.Id };

            dataContextMock.Expect(mock => mock.FindById<GamingGroup>(gamingGroupId, currentUser))
                .Return(expectedGamingGroup);

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            applicationUsers.Add(currentUser);

            dataContextMock.Expect(mock => mock.GetQueryable<ApplicationUser>(currentUser))
                .Return(applicationUsers.AsQueryable());

            List<GamingGroupInvitation> gamingGroupInvitations = new List<GamingGroupInvitation>();
            expectedGamingGroupInvitation = new GamingGroupInvitation() 
            { 
                GamingGroupId = expectedGamingGroup.Id, 
                RegisteredUserId = currentUser.Id 
            };
            
            gamingGroupInvitations.Add(expectedGamingGroupInvitation);
            dataContextMock.Expect(mock => mock.GetQueryable<GamingGroupInvitation>(currentUser))
                .Return(gamingGroupInvitations.AsQueryable());

            dataContextMock.Expect(mock => mock.GetQueryable<ApplicationUser>(currentUser))
                .Return(applicationUsers.AsQueryable());
        }

        [Test]
        public void ItReturnsTheGamingGroup()
        {
            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, currentUser);

            Assert.AreSame(expectedGamingGroup, actualGamingGroup);
        }

        [Test]
        public void ItReturnsTheOwningUserOnTheGameDefinition()
        {
            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, currentUser);

            Assert.NotNull(actualGamingGroup.OwningUser);
        }

        [Test]
        public void ItReturnsTheGamingGroupInvitationsOnTheGamingGroup()
        {
            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, currentUser);

            Assert.AreSame(expectedGamingGroup.GamingGroupInvitations[0], actualGamingGroup.GamingGroupInvitations[0]);
        }
        
        [Test]
        public void ItReturnsTheRegisteredUserNameOfAnyUsersThatRegistered()
        {
            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, currentUser);

            foreach(GamingGroupInvitation invitation in actualGamingGroup.GamingGroupInvitations)
            {
                if(!string.IsNullOrEmpty(invitation.RegisteredUserId))
                {
                    Assert.NotNull(invitation.RegisteredUser);
                }
            }
        }
    }
}
