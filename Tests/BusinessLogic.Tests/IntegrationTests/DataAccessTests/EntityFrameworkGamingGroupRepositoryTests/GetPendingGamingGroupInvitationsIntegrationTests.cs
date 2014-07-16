using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using BusinessLogic.Tests.IntegrationTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.EntityFrameworkGamingGroupRepositoryTests
{
    [TestFixture]
    public class GetPendingGamingGroupInvitationsIntegrationTests : EntityFrameworkGamingGroupRepositoryIntegrationTestBase
    {
        [Test]
        public void ItGetsGamingGroupInvitationsForTheCurrentUsersEmailAddress()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                EntityFrameworkGamingGroupRepository repository = new EntityFrameworkGamingGroupRepository(dbContext);

                IList<GamingGroupInvitation> invitations = repository.GetPendingGamingGroupInvitations(testUserContextForUserWithDefaultGamingGroup);

                Assert.True(invitations.All(invitation => invitation.InviteeEmail == testUserWithDefaultGamingGroup.Email));
            }
        }

        [Test]
        public void ItThrowsAKeyNotFoundExceptionIfTheUserDoesNotExist()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                UserContext userThatDoesntExist = new UserContext()
                {
                    ApplicationUserId = "this doesn't exist"
                };
                EntityFrameworkGamingGroupRepository repository = new EntityFrameworkGamingGroupRepository(dbContext);

                Exception exception = Assert.Throws<KeyNotFoundException>(() => repository.GetPendingGamingGroupInvitations(userThatDoesntExist));

                string expectedMessage = string.Format(EntityFrameworkGamingGroupRepository.EXCEPTION_MESSAGE_USER_DOES_NOT_EXIST, userThatDoesntExist.ApplicationUserId);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }

        [Test]
        public void ItReturnsAnEmptyListIfThereAreNoInvitations()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                EntityFrameworkGamingGroupRepository repository = new EntityFrameworkGamingGroupRepository(dbContext);
                UserContext userWithNoInvites = new UserContext() { ApplicationUserId = testUserWithDefaultGamingGroupAndNoInvites.Id };
                IList<GamingGroupInvitation> invitations = repository.GetPendingGamingGroupInvitations(userWithNoInvites);

                Assert.AreEqual(0, invitations.Count);
            }
        }
    }
}
