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
        private IDataContext dataContextMock;
        private IPlayerRepository playerRepositoryMock;
        private NemesisRecalculator nemesisRecalculatorPartialMock;
        private IQueryable<Player> allPlayersQueryable;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            nemesisRecalculatorPartialMock = MockRepository.GeneratePartialMock<NemesisRecalculator>(dataContextMock, playerRepositoryMock);

            List<Player> allPlayers = new List<Player>()
            {
                new Player(){ Active = true, Id = 1 },
                new Player(){ Active = true, Id = 2 },
                new Player(){ Active = false, Id = 3 }
            };

            allPlayersQueryable = allPlayers.AsQueryable();

            dataContextMock.Expect(mock => mock.GetQueryable<Player>())
                .Return(allPlayersQueryable);
        }

        [Test]
        public void ItRecalculatesTheNemesisForEachActivePlayerInTheGamingGroupUsingAFakeUserThatHasAccessToThatPlayersGamingGroup()
        {
            List<Player> activePlayersOnly = allPlayersQueryable.Where(player => player.Active == true).ToList();
            nemesisRecalculatorPartialMock.Expect(mock => mock.RecalculateNemesis(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            nemesisRecalculatorPartialMock.RecalculateAllNemeses();

            foreach(Player activePlayer in activePlayersOnly)
            {
                nemesisRecalculatorPartialMock.AssertWasCalled(mock => mock.RecalculateNemesis(
                    Arg<int>.Is.Equal(activePlayer.Id), 
                    Arg<ApplicationUser>.Matches(appUser => appUser.CurrentGamingGroupId == activePlayer.GamingGroupId)));
            }
        }
    }
}
