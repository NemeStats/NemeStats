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
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.SecurityTests.SecuredEntityValidatorTests
{
    [TestFixture]
    public class ValidateAccessTests
    {
        private RhinoAutoMocker<SecuredEntityValidator<SecuredEntityWithTechnicalKey>> _autoMocker; 
        protected SecuredEntityValidator<object> securedEntityValidatorForEntityThatIsNotSecured;
        protected SecuredEntityWithTechnicalKey securedEntity;
        protected ApplicationUser currentUser;
        protected int securedEntityGamingGroupId = 1;
        protected int securedEntityId = 9;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<SecuredEntityValidator<SecuredEntityWithTechnicalKey>>();
            securedEntityValidatorForEntityThatIsNotSecured = new SecuredEntityValidator<object>(_autoMocker.Get<IDataContext>());
            securedEntity = MockRepository.GenerateMock<SecuredEntityWithTechnicalKey>();
            currentUser = new ApplicationUser();

            securedEntity.Expect(mock => mock.GamingGroupId)
                 .Repeat.Any()
                 .Return(securedEntityGamingGroupId);
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheUserDoesNotHaveAccessToTheGamingGroup()
        {
            currentUser.CurrentGamingGroupId = 999999;
            
            var expectedException = new UnauthorizedEntityAccessException(currentUser.Id,
                typeof(SecuredEntityWithTechnicalKey),
                string.Empty);
            var userGamingGroupQueryable = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    GamingGroupId = securedEntity.GamingGroupId - 1,
                    ApplicationUserId = currentUser.Id
                },
                new UserGamingGroup
                {
                    GamingGroupId = securedEntity.GamingGroupId,
                    ApplicationUserId = currentUser.Id + "something to make it not hit"
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>())
                       .Return(userGamingGroupQueryable);

            var exception = Assert.Throws<UnauthorizedEntityAccessException>(
                () => _autoMocker.ClassUnderTest.ValidateAccess(securedEntity, currentUser, string.Empty));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheEntityIsNotASecuredEntityWithTechnicalKey()
        {
            securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess(
                "some object that doesnt extend SecuredEntityWithTechnicalKey", 
                currentUser,
                string.Empty);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheEntityDoesNotHaveAGamingGroupIdSet()
        {
            currentUser.CurrentGamingGroupId = 50;

            securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess(
                new Player(),
                currentUser,
                string.Empty);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheUsersCurrentGamingGroupIsThatOfTheSecuredEntity()
        {
            currentUser.CurrentGamingGroupId = securedEntity.GamingGroupId;

            securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess(
                securedEntity,
                currentUser,
                string.Empty);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheUserHasTheGamingGroupOfTheSecuredEntity()
        {
            var userGamingGroupQueryable = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    GamingGroupId = securedEntity.GamingGroupId,
                    ApplicationUserId = currentUser.Id
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>())
                       .Return(userGamingGroupQueryable);

            securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess(
                securedEntity,
                currentUser,
                string.Empty);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheCurrentUserIsNullAndTheEntityIsSecured()
        {
            Assert.Throws<ArgumentNullException>(
                () => _autoMocker.ClassUnderTest.ValidateAccess(
                    securedEntity, 
                    null,                    
                    string.Empty));
        }
    }
}
