using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayerRepositoryTests
{
    [TestFixture]
    public class GetNemesisTests
    {
        private EntityFrameworkPlayerRepository playerRepositoryPartialMock;
        private NemeStatsDbContext dbContext;
        private UserContext userContext;

        [SetUp]
        public void SetUp()
        {
            dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            playerRepositoryPartialMock = MockRepository.GeneratePartialMock<EntityFrameworkPlayerRepository>(dbContext);
            userContext = new UserContext()
            {
                GamingGroupId = 1881
            };
        }

        [Test]
        public void ItThrowsAnUnauthorizedExceptionIfTheUserDoesntHaveAccessToThePlayer()
        {
            int playerId = 123;
            playerRepositoryPartialMock.Expect(partialMock => partialMock.GetPlayer(playerId, userContext))
                .Throw(new UnauthorizedAccessException());

            Assert.Throws<UnauthorizedAccessException>(() => playerRepositoryPartialMock.GetNemesis(playerId, userContext));
        }
    }
}
