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
        private ISecuredEntityValidator _securedEntityValidator;
        private DbSet<GameDefinition> _gameDefinitionDbSetMock;
        private GameDefinition _gameDefinition;

        private int _gameDefinitionId = 1;

        [SetUp]
        public void SetUp()
        {
            _gameDefinitionDbSetMock = MockRepository.GenerateMock<DbSet<GameDefinition>>();

            nemeStatsDbContext.Expect(mock => mock.Set<GameDefinition>())
                .Repeat.Once()
                .Return(_gameDefinitionDbSetMock);

            _securedEntityValidator = MockRepository.GenerateMock<ISecuredEntityValidator>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<GameDefinition>(dataContext))
                .Repeat.Once()
                .Return(_securedEntityValidator);

            _gameDefinition = new GameDefinition
            {
                Id = _gameDefinitionId
            };
            _securedEntityValidator.Expect(mock => mock.ValidateAccess<GameDefinition>(_gameDefinitionId, currentUser))
                .Return(_gameDefinition);
        }

        [Test]
        public void ItDeletesTheSpecifiedEntity()
        {
            //--act
            dataContext.DeleteById<GameDefinition>(_gameDefinitionId, currentUser);

            _gameDefinitionDbSetMock.AssertWasCalled(mock => mock.Remove(_gameDefinition));
        }

        [Test]
        public void ItValidatesAccessToTheEntity()
        {
            //--arrange
            var entityId = 1;

            //--act
            dataContext.DeleteById<GameDefinition>(entityId, currentUser);

            //--assert
            _securedEntityValidator.AssertWasCalled(mock => mock.ValidateAccess<GameDefinition>(
                Arg<int>.Is.Equal(entityId), 
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }
    }
}
