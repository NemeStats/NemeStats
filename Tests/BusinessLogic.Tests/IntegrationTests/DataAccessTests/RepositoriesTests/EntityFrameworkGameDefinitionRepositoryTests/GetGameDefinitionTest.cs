using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkGameDefinitionRepositoryTests
{
    [TestFixture]
    public class GetGameDefinitionTest : EntityFrameworkGameDefinitionRepositoryTestBase
    {
        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheCurrentUsersGamingGroupDoesNotMatch()
        {
            UnauthorizedEntityAccessException expectedException = new UnauthorizedEntityAccessException(
                testUserWithDefaultGamingGroup.Id,
                testGameDefinitionWithOtherGamingGroupId.Id);

            UnauthorizedEntityAccessException actualException = Assert.Throws<UnauthorizedEntityAccessException>(
                () => gameDefinitionRepository.GetGameDefinition(
                    testGameDefinitionWithOtherGamingGroupId.Id,
                    testUserWithDefaultGamingGroup));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAKeyNotFoundExceptionIfNoGameDefinitionIsFoundForThatId()
        {
            int invalidId = -1;
            EntityDoesNotExistException expectedException = new EntityDoesNotExistException(invalidId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException>(() => gameDefinitionRepository.GetGameDefinition(
                invalidId,
                testUserWithDefaultGamingGroup));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItReturnsTheSpecifiedGameDefinition()
        {
            GameDefinition gameDefinition = gameDefinitionRepository.GetGameDefinition(testGameDefinition.Id,
                
                testUserWithDefaultGamingGroup);

            Assert.AreEqual(testGameDefinition.Id, gameDefinition.Id);
        }
    }
}
