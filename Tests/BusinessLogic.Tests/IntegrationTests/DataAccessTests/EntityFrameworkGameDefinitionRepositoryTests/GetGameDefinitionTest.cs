using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests
{
    [TestFixture]
    public class GetGameDefinitionTest : EntityFrameworkGameDefinitionRepositoryTestBase
    {
        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheCurrentUsersGamingGroupDoesNotMatch()
        {
            Exception exception = Assert.Throws<UnauthorizedAccessException>(() => gameDefinitionRepository.GetGameDefinition(
                testGameDefinitionWithOtherGamingGroupId.Id,
                dbContext, 
                testUserContextForUserWithDefaultGamingGroup));

            string message = string.Format(
                EntityFrameworkGameDefinitionRepository.EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION,
                testUserContextForUserWithDefaultGamingGroup.ApplicationUserId,
                testGameDefinitionWithOtherGamingGroupId.Id);

            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void ItThrowsAKeyNotFoundExceptionIfNoGameDefinitionIsFoundForThatId()
        {
            int invalidId = -1;

            Exception exception = Assert.Throws<KeyNotFoundException>(() => gameDefinitionRepository.GetGameDefinition(
                invalidId,
                dbContext,
                testUserContextForUserWithDefaultGamingGroup));

            string message = string.Format(
                EntityFrameworkGameDefinitionRepository.EXCEPTION_MESSAGE_GAME_DEFINITION_NOT_FOUND,
                invalidId.ToString());

            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void ItReturnsTheSpecifiedGameDefinition()
        {
            GameDefinition gameDefinition = gameDefinitionRepository.GetGameDefinition(testGameDefinition.Id,
                dbContext,
                testUserContextForUserWithDefaultGamingGroup);

            Assert.AreEqual(testGameDefinition.Id, gameDefinition.Id);
        }
    }
}
