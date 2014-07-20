using BusinessLogic.DataAccess.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.EntityFrameworkGamingGroupRepositoryTests
{
    [TestFixture]
    public class GetGamingGroupDetailsTests : EntityFrameworkGamingGroupRepositoryTestBase
    {
        [Test]
        public void ItThrowsAnUnauthorizedExceptionIfTheUserDoesntHaveAccess()
        {
            int gamingGroupId = -1;

            Exception exception = Assert.Throws<UnauthorizedAccessException>(
                () => gamingGroupRepositoryPartialMock.GetGamingGroupDetails(gamingGroupId, currentUser));

            string expectedMessage = string.Format(EntityFrameworkGamingGroupRepository.EXCEPTION_MESSAGE_NO_ACCESS_TO_GAMING_GROUP,
                currentUser.Id,
                gamingGroupId);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}
