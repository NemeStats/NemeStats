using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    public class PlayerRetrieverTestBase
    {
        internal PlayerRetriever playerRetriever;
        internal IDataContext dataContextMock;
        internal IQueryable<Player> playerQueryable;
        internal int gamingGroupId = 558585;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRetriever = new PlayerRetriever(dataContextMock);

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
