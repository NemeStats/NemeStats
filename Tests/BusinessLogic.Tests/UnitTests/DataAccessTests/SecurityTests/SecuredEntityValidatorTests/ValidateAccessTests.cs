using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Exceptions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.SecurityTests.SecuredEntityValidatorTests
{
    [TestFixture]
    public class ValidateAccessTests
    {
        protected SecuredEntityValidator<SecuredEntityWithTechnicalKey> securedEntityValidatorForSecuredEntity;
        protected SecuredEntityValidator<object> securedEntityValidatorForEntityThatIsNotSecured;
        protected SecuredEntityWithTechnicalKey securedEntity;
        protected ApplicationUser currentUser;
        protected int securedEntityGamingGroupId = 1;
        protected int securedEntityId = 9;

        [SetUp]
        public void SetUp()
        {
            securedEntityValidatorForSecuredEntity = new SecuredEntityValidator<SecuredEntityWithTechnicalKey>();
            securedEntityValidatorForEntityThatIsNotSecured = new SecuredEntityValidator<object>();
            securedEntity = MockRepository.GenerateMock<SecuredEntityWithTechnicalKey>();
            currentUser = new ApplicationUser();

            securedEntity.Expect(mock => mock.GamingGroupId)
                 .Repeat.Any()
                 .Return(securedEntityGamingGroupId);
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheGamingGroupIdsDoNotMatch()
        {
            currentUser.CurrentGamingGroupId = 999999;
            Type stringType = typeof(string);
            UnauthorizedEntityAccessException expectedException = new UnauthorizedEntityAccessException(currentUser.Id,
                stringType,
                string.Empty);

            UnauthorizedEntityAccessException exception = Assert.Throws<UnauthorizedEntityAccessException>(
                () => securedEntityValidatorForSecuredEntity.ValidateAccess(securedEntity, currentUser, stringType, string.Empty));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheEntityIsNotASecuredEntityWithTechnicalKey()
        {
            securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess(
                "some object that doesnt extend SecuredEntityWithTechnicalKey", 
                currentUser, 
                typeof(string),
                string.Empty);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheUserHasTheGamingGroupOfTheSecuredEntity()
        {
            currentUser.CurrentGamingGroupId = securedEntity.GamingGroupId;

            securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess(
                securedEntity,
                currentUser,
                typeof(string),
                string.Empty);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheCurrentUserIsNullAndTheEntityIsSecured()
        {
            Exception exception = Assert.Throws<ArgumentNullException>(
                () => securedEntityValidatorForSecuredEntity.ValidateAccess(
                    securedEntity, 
                    null, 
                    typeof(string), 
                    string.Empty));
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGamingGroupIdIsNullAndTheEntityIsSecured()
        {
            Exception exception = Assert.Throws<ArgumentException>(
                () => securedEntityValidatorForSecuredEntity.ValidateAccess(
                    securedEntity, 
                    new ApplicationUser(), 
                    typeof(string), 
                    string.Empty));

            Assert.AreEqual(
                SecuredEntityValidator<SecuredEntityWithTechnicalKey>.EXCEPTION_MESSAGE_CURRENT_USER_GAMING_GROUP_ID_CANNOT_BE_NULL, 
                exception.Message);
        }
    }
}
