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

using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.ChampionsTests.ChampionRecalculatorTests
{
    [TestFixture]
    public class RecalculateAllChampionsTests
    {
        private IDataContext _dataContextMock;
        private IChampionRepository _championRepositoryMock;
        private ChampionRecalculator _championRecalculatorPartialMock;
        private IQueryable<GameDefinition> _allGameDefinitionsQueryable;

        [SetUp]
        public void SetUp()
        {
            _dataContextMock = MockRepository.GenerateMock<IDataContext>();
            _championRepositoryMock = MockRepository.GenerateMock<IChampionRepository>();
            _championRecalculatorPartialMock = MockRepository.GeneratePartialMock<ChampionRecalculator>(_dataContextMock,
                _championRepositoryMock);

            List<GameDefinition> allGameDefinitions = new List<GameDefinition>
            {
                new GameDefinition { Active = true, Id = 1 },
                new GameDefinition { Active = true, Id = 2 },
                new GameDefinition { Active = false, Id = 3 }
            };

            _allGameDefinitionsQueryable = allGameDefinitions.AsQueryable();

            _dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(_allGameDefinitionsQueryable);
        }

        [Test]
        public void ItCalculatesTheChampionForEachActiveGameDefinition()
        {
            List<GameDefinition> activeGameDefinitions =
                _allGameDefinitionsQueryable.Where(gameDefinition => gameDefinition.Active).ToList();
            _championRecalculatorPartialMock.Expect(mock => mock.RecalculateChampion(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything, Arg<IDataContext>.Is.Anything, Arg<bool>.Is.Anything))
                .Return(new Champion());

            _championRecalculatorPartialMock.RecalculateAllChampions();

            foreach (GameDefinition gameDefinition in activeGameDefinitions)
            {
                _championRecalculatorPartialMock.AssertWasCalled(mock => 
                    mock.RecalculateChampion(Arg<int>.Is.Equal(gameDefinition.Id), Arg<ApplicationUser>.Is.Anything, Arg<IDataContext>.Is.Anything, Arg<bool>.Is.Equal(true)));
            }
        }
    }
}
