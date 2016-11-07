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
        private RhinoAutoMocker<SecuredEntityValidator> _autoMocker;
        private SecuredEntityValidator _securedEntityValidatorForEntityThatIsNotSecured;
        private GameDefinition _securedEntity;
        private ApplicationUser _currentUser;
        private int _securedEntityGamingGroupId = 1;
        private int _securedEntityId = 9;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<SecuredEntityValidator>();
            _securedEntityValidatorForEntityThatIsNotSecured = new SecuredEntityValidator(_autoMocker.Get<IDataContext>());
            _securedEntity = new GameDefinition
            {
                Id = _securedEntityId,
                GamingGroupId = _securedEntityGamingGroupId
            };
            _currentUser = new ApplicationUser();

            _autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<GameDefinition>(_securedEntityId))
                .Return(_securedEntity);
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheUserDoesNotHaveAccessToTheGamingGroup()
        {
            _currentUser.CurrentGamingGroupId = 999999;
            
            var expectedException = new UnauthorizedEntityAccessException(_currentUser.Id,
                _securedEntity.GetType(),
                _securedEntityId);
            var userGamingGroupQueryable = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    GamingGroupId = _securedEntity.GamingGroupId - 1,
                    ApplicationUserId = _currentUser.Id
                },
                new UserGamingGroup
                {
                    GamingGroupId = _securedEntity.GamingGroupId,
                    ApplicationUserId = _currentUser.Id + "something to make it not hit"
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>())
                       .Return(userGamingGroupQueryable);

            var exception = Assert.Throws<UnauthorizedEntityAccessException>(
                () => _autoMocker.ClassUnderTest.ValidateAccess<GameDefinition>(_securedEntity.Id, _currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheEntityDoesNotHaveAGamingGroupIdSet()
        {
            _currentUser.CurrentGamingGroupId = 50;
            _securedEntity.GamingGroupId = 0;

            _securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess<GameDefinition>(_securedEntityId, _currentUser);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheUsersCurrentGamingGroupIsThatOfTheSecuredEntity()
        {
            _currentUser.CurrentGamingGroupId = _securedEntity.GamingGroupId;

            _securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess<GameDefinition>(_securedEntity.Id, _currentUser);
        }

        [Test]
        public void ItDoesNotThrowAnExceptionIfTheUserHasTheGamingGroupOfTheSecuredEntity()
        {
            var userGamingGroupQueryable = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    GamingGroupId = _securedEntity.GamingGroupId,
                    ApplicationUserId = _currentUser.Id
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>())
                       .Return(userGamingGroupQueryable);

            _securedEntityValidatorForEntityThatIsNotSecured.ValidateAccess<GameDefinition>(_securedEntity.Id, _currentUser);

        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheCurrentUserIsNullAndTheEntityIsSecured()
        {
            Assert.Throws<ArgumentNullException>(
                () => _autoMocker.ClassUnderTest.ValidateAccess<GameDefinition>(_securedEntity.Id, null));
        }
    }
}
