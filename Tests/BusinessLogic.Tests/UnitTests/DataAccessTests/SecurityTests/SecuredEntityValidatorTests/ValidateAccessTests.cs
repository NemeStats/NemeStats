using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.SecurityTests.SecuredEntityValidatorTests
{
    [TestFixture]
    public class ValidateAccessTests
    {
        protected SecuredEntityValidatorImpl<SecuredEntityWithTechnicalKey> securedEntityValidatorForSecuredEntity;
        protected SecuredEntityValidatorImpl<object> securedEntityValidatorForEntityThatIsNotSecured;
        protected SecuredEntityWithTechnicalKey securedEntity;
        protected ApplicationUser currentUser;
        protected int securedEntityGamingGroupId = 1;
        protected int securedEntityId = 9;

        [SetUp]
        public void SetUp()
        {
            securedEntityValidatorForSecuredEntity = new SecuredEntityValidatorImpl<SecuredEntityWithTechnicalKey>();
            securedEntityValidatorForEntityThatIsNotSecured = new SecuredEntityValidatorImpl<object>();
            securedEntity = MockRepository.GenerateMock<SecuredEntityWithTechnicalKey>();
            currentUser = new ApplicationUser();

            securedEntity.Expect(mock => mock.GamingGroupId)
                 .Repeat.Any()
                 .Return(securedEntityGamingGroupId);
            securedEntity.Expect(mock => mock.Id)
                .Repeat.Any()
                .Return(securedEntityId);
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheGamingGroupIdsDoNotMatch()
        {
            currentUser.CurrentGamingGroupId = 999999;
            Type stringType = typeof(string);

            Exception exception = Assert.Throws<UnauthorizedAccessException>(
                () => securedEntityValidatorForSecuredEntity.ValidateAccess(securedEntity, currentUser, stringType));

            string message = string.Format(
                SecuredEntityValidatorImpl<SecuredEntityWithTechnicalKey>.EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION,
                currentUser.Id,
                stringType,
                securedEntity.Id
                );
            Assert.AreEqual(message, exception.Message);
        }
    }
}
