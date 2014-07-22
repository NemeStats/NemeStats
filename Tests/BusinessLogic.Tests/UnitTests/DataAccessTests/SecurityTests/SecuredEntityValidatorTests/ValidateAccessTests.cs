using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
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
        protected SecuredEntityValidatorImpl securedEntityValidator;
        protected SecuredEntityWithTechnicalKey securedEntity;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            securedEntityValidator = new SecuredEntityValidatorImpl();
            securedEntity = MockRepository.GenerateMock<SecuredEntityWithTechnicalKey>();
            currentUser = new ApplicationUser();
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheGamingGroupIdsDoNotMatch()
        {
            int securedEntityGamingGroupId = 1;
            currentUser.CurrentGamingGroupId = 999999;
            Type stringType = typeof(string);
            securedEntity.Expect(mock => mock.GamingGroupId)
                .Repeat.Once()
                .Return(securedEntityGamingGroupId);

            Exception exception = Assert.Throws<UnauthorizedAccessException>(
                () => securedEntityValidator.ValidateAccess(securedEntity, currentUser, stringType));

            string message = string.Format(
                SecuredEntityValidatorImpl.EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION,
                currentUser.Id,
                stringType,
                currentUser
                );
            Assert.AreEqual(message, exception.Message);
        }
    }
}
