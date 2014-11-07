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
