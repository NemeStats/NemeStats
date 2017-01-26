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
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.NemesesTests.NemesisRecalculatorTests
{
    [TestFixture]
    public class RecalculateAllNemesesTests
    {
        private IDataContext _dataContextMock;
        private IPlayerRepository _playerRepositoryMock;
        private NemesisRecalculator _nemesisRecalculatorPartialMock;
        private IQueryable<Player> _allPlayersQueryable;

        [SetUp]
        public void SetUp()
        {
            _dataContextMock = MockRepository.GenerateMock<IDataContext>();
            _playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            _nemesisRecalculatorPartialMock = MockRepository.GeneratePartialMock<NemesisRecalculator>(_dataContextMock, _playerRepositoryMock);

            var allPlayers = new List<Player>()
            {
                new Player(){ Active = true, Id = 1 },
                new Player(){ Active = true, Id = 2 },
                new Player(){ Active = false, Id = 3 }
            };

            _allPlayersQueryable = allPlayers.AsQueryable();

            _dataContextMock.Expect(mock => mock.GetQueryable<Player>())
                .Return(_allPlayersQueryable);
        }

        [Test]
        public void ItRecalculatesTheNemesisForEachActivePlayerInTheGamingGroupUsingAFakeUserThatHasAccessToThatPlayersGamingGroup()
        {
            var activePlayersOnly = _allPlayersQueryable.Where(player => player.Active == true).ToList();
            _nemesisRecalculatorPartialMock.Expect(mock => mock.RecalculateNemesis(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything, Arg<IDataContext>.Is.Anything));

            _nemesisRecalculatorPartialMock.RecalculateAllNemeses();

            foreach(var activePlayer in activePlayersOnly)
            {
                _nemesisRecalculatorPartialMock.AssertWasCalled(mock => mock.RecalculateNemesis(
                    Arg<int>.Is.Equal(activePlayer.Id), 
                    Arg<ApplicationUser>.Matches(appUser => appUser.CurrentGamingGroupId == activePlayer.GamingGroupId),
                    Arg<IDataContext>.Is.Anything));
            }
        }
    }
}
