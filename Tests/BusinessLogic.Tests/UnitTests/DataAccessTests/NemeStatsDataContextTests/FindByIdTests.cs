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
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class FindByIdTests : NemeStatsDataContextTestBase
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
        public void ItReturnsTheEntityForTheGivenId()
        {
            int entityId = 1;
            GameDefinition expectedGameDefinition = new GameDefinition() { Id = entityId };
            gameDefinitionDbSetMock.Expect(mock => mock.Find(entityId))
                .Return(expectedGameDefinition);

            GameDefinition actualGameDefinition = dataContext.FindById<GameDefinition>(entityId);

            Assert.AreSame(expectedGameDefinition, actualGameDefinition);
        }

        [Test]
        public void ItValidatesThatTheEntityExists()
        {
            int entityId = 1;
            GameDefinition gameDefinition = new GameDefinition() { Id = entityId };
            gameDefinitionDbSetMock.Expect(mock => mock.Find(entityId))
                .Return(gameDefinition);

            dataContext.FindById<GameDefinition>(entityId);

            dataContext.AssertWasCalled(mock => mock.ValidateEntityExists<GameDefinition>(entityId, gameDefinition));
        }
    }
}
