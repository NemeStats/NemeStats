using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BatchTests.NemesisRecalculatorTests
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
