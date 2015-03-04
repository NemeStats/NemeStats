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
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    public class PlayerRetrieverTestBase
    {
        internal PlayerRetriever playerRetriever;
        internal IDataContext dataContextMock;
        internal IQueryable<Player> playerQueryable;
        internal int gamingGroupId = 558585;
        internal IPlayerRepository playerRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            playerRetriever = new PlayerRetriever(dataContextMock, playerRepositoryMock);

            List<Player> players = new List<Player>()
            {
                new Player(){ GamingGroupId = gamingGroupId, Name = "2" },
                new Player(){ GamingGroupId = gamingGroupId, Name = "3" },
                new Player(){ GamingGroupId = -1, Name = "1" },
                new Player(){ GamingGroupId = gamingGroupId, Name = "1" },
            };
            players[3].NemesisId = 1;
            players[3].Nemesis = new Nemesis()
            {
                NemesisPlayerId = 2,
                NemesisPlayer = new Player() { Id = 93995 }
            };
            playerQueryable = players.AsQueryable<Player>();

            dataContextMock.Expect(mock => mock.GetQueryable<Player>())
                .Return(playerQueryable);
        }
    }
}
