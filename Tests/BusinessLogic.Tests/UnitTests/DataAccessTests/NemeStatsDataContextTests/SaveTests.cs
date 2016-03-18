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

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class SaveTests : NemeStatsDataContextTestBase
    {
        protected ISecuredEntityValidator<EntityWithTechnicalKey> securedEntityValidator;

        [SetUp]
        public void SetUp()
        {
            securedEntityValidator = MockRepository.GenerateMock<ISecuredEntityValidator<EntityWithTechnicalKey>>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<EntityWithTechnicalKey>(dataContext))
                .Repeat.Once()
                .Return(securedEntityValidator);
        }

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

            Exception actualException = Assert.Throws<ArgumentNullException>(() => dataContext.Save<GamingGroup>(new GamingGroup(), null));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItValidatesSecurityIfTheEntityIsAlreadyInTheDatabase()
        {
            entityWithGamingGroup.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            securedEntityValidator.Expect(mock => mock.ValidateAccess(
                entityWithGamingGroup, 
                currentUser, 
                typeof(EntityWithTechnicalKey),
                NemeStatsDataContext.UNKNOWN_ENTITY_ID))
                .Throw(new UnauthorizedAccessException());
            try
            {
                dataContext.Save<EntityWithTechnicalKey>(entityWithGamingGroup, currentUser);
            }
            catch (UnauthorizedAccessException) { }

            securedEntityValidator.VerifyAllExpectations();
        }

        [Test]
        public void ItAddsOrSavesTheEntity()
        {
            entityWithGamingGroup.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            dataContext.Expect(mock => mock.AddOrInsertOverride<EntityWithTechnicalKey>(entityWithGamingGroup))
                .Repeat.Once()
                .Return(entityWithGamingGroup);

            dataContext.Save<EntityWithTechnicalKey>(entityWithGamingGroup, currentUser);

            dataContext.AssertWasCalled(mock => mock.AddOrInsertOverride<EntityWithTechnicalKey>(entityWithGamingGroup));
        }

        [Test]
        public void ItSetsTheGamingGroupIdIfItIsASecuredEntityThatIsntAlreadyInTheDatabase()
        {
            GameDefinition gameDefinition = new GameDefinition();

            dataContext.Expect(mock => mock.AddOrInsertOverride(gameDefinition))
                .Repeat.Once()
                .Return(gameDefinition);

            dataContext.Save(gameDefinition, currentUser);

            dataContext.AssertWasCalled(mock => mock.AddOrInsertOverride(
                Arg<GameDefinition>.Matches(entity => entity.GamingGroupId == currentUser.CurrentGamingGroupId)));
        }
    }
}
