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
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;
using BusinessLogic.Exceptions;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class SaveTests : NemeStatsDataContextTestBase
    {
        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheEntityIsNull()
        {
            Exception expectedException = new ArgumentNullException("entity");

            Exception actualException = Assert.Throws<ArgumentNullException>(() => dataContext.Save<GamingGroup>(null, new ApplicationUser()));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheCurrentUserIsNull()
        {
            Exception expectedException = new ArgumentNullException("currentUser");

            Exception actualException = Assert.Throws<ArgumentNullException>(() => dataContext.Save(new GamingGroup(), null));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAUserHasNoGamingGroupExceptionIfTheCurrentUserHasNoGamingGroup()
        {
            var user = new ApplicationUser
            {
                Id = "some user id"
            };
            var expectedException = new UserHasNoGamingGroupException(user.Id);

            var actualException = Assert.Throws<UserHasNoGamingGroupException>(() => dataContext.Save(new GamingGroup(), user));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItValidatesThatTheCurrentUserHasAccessToSaveTheEntityInOneOfTheirGamingGroups()
        {
            //--arrange
            entityWithGamingGroup.Expect(mock => mock.AlreadyInDatabase()).Return(true);

            var securedEntityValidator = MockRepository.GenerateMock<ISecuredEntityValidator>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<IEntityWithTechnicalKey>(dataContext))
                .IgnoreArguments()
                .Return(securedEntityValidator);
            dataContext.Expect(mock => mock.AddOrInsertOverride(Arg<IEntityWithTechnicalKey>.Is.Anything)).Return(entityWithGamingGroup);

            //--act
            dataContext.Save(entityWithGamingGroup, currentUser);

            //--assert
            securedEntityValidator.AssertWasCalled(mock => mock.ValidateAccess<IEntityWithTechnicalKey>(entityWithGamingGroup, currentUser));
        }

        [Test]
        public void ItAddsOrSavesTheEntity()
        {
            //--arrange
            entityWithGamingGroup.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            dataContext.Expect(mock => mock.AddOrInsertOverride(entityWithGamingGroup))
                .Repeat.Once()
                .Return(entityWithGamingGroup);

            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<IEntityWithTechnicalKey>(dataContext))
                .Return(MockRepository.GenerateMock<ISecuredEntityValidator>());

            //--act
            dataContext.Save(entityWithGamingGroup, currentUser);

            //--assert
            dataContext.AssertWasCalled(mock => mock.AddOrInsertOverride(entityWithGamingGroup));
        }

        [Test]
        public void ItSetsTheGamingGroupIdIfItIsASecuredEntityThatIsntAlreadyInTheDatabaseAndTheGamingGroupIdIsntAlreadySet()
        {
            var gameDefinition = new GameDefinition();

            var securedEntityValidator = MockRepository.GenerateMock<ISecuredEntityValidator>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<GameDefinition>(dataContext))
                .IgnoreArguments()
                .Return(securedEntityValidator);
            securedEntityValidator.Expect(mock => mock.ValidateAccess<GameDefinition>(null, null)).IgnoreArguments();

            dataContext.Expect(mock => mock.AddOrInsertOverride(gameDefinition))
                .Repeat.Once()
                .Return(gameDefinition);

            dataContext.Save(gameDefinition, currentUser);

            dataContext.AssertWasCalled(mock => mock.AddOrInsertOverride(
                Arg<GameDefinition>.Matches(entity => entity.GamingGroupId == currentUser.CurrentGamingGroupId)));
        }

        [Test]
        public void ItDoesntSetTheGamingGroupIdIfTheEntityIsAGamingGroupItself()
        {
            var gamingGroup = new GamingGroup();

            var securedEntityValidator = MockRepository.GenerateMock<ISecuredEntityValidator>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<GamingGroup>(dataContext))
                .IgnoreArguments()
                .Return(securedEntityValidator);
            securedEntityValidator.Expect(mock => mock.ValidateAccess<GameDefinition>(null, null)).IgnoreArguments();

            dataContext.Expect(mock => mock.AddOrInsertOverride(gamingGroup))
                .Repeat.Once()
                .Return(gamingGroup);

            dataContext.Save(gamingGroup, currentUser);

            //--GamingGroup.GamingGroupId is just an alias for GamingGroup.Id, so this should remain the same
            dataContext.AssertWasNotCalled(mock => mock.AddOrInsertOverride(
                Arg<GamingGroup>.Matches(entity => entity.GamingGroupId == currentUser.CurrentGamingGroupId)));
        }
    }
}
