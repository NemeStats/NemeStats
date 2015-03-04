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
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class DeleteByIdTests : NemeStatsDataContextTestBase
    {
        private ISecuredEntityValidator<GameDefinition> securedEntityValidator;
        private DbSet<GameDefinition> gameDefinitionDbSetMock;

        [SetUp]
        public void SetUp()
        {
            gameDefinitionDbSetMock = MockRepository.GenerateMock<DbSet<GameDefinition>>();

            nemeStatsDbContext.Expect(mock => mock.Set<GameDefinition>())
                .Repeat.Once()
                .Return(gameDefinitionDbSetMock);

            securedEntityValidator = MockRepository.GenerateMock<ISecuredEntityValidator<GameDefinition>>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<GameDefinition>())
                .Repeat.Once()
                .Return(securedEntityValidator);
        }

        [Test]
        public void ItDeletesTheSpecifiedEntity()
        {
            int id = 1;
            GameDefinition gameDefinition = new GameDefinition() { Id = id };
            dataContext.Expect(mock => mock.FindById<GameDefinition>(id))
                .Return(gameDefinition);

            dataContext.DeleteById<GameDefinition>(id, currentUser);
            dataContext.CommitAllChanges();

            gameDefinitionDbSetMock.AssertWasCalled(mock => mock.Remove(gameDefinition));
        }

        [Test]
        public void ItValidatesAccessToTheEntity()
        {
            int entityId = 1;
            GameDefinition gameDefinition = new GameDefinition() { Id = entityId };
            dataContext.Expect(mock => mock.FindById<GameDefinition>(entityId))
                .Return(gameDefinition);

            dataContext.DeleteById<GameDefinition>(entityId, currentUser);
            //TODO should probably check each 
            securedEntityValidator.AssertWasCalled(mock => mock.ValidateAccess(
                Arg<GameDefinition>.Is.Same(gameDefinition), 
                Arg<ApplicationUser>.Is.Same(currentUser),
                Arg<Type>.Is.Equal(typeof(GameDefinition)),
                Arg<int>.Is.Equal(entityId)));
        }
    }
}
