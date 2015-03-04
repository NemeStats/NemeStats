#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
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
