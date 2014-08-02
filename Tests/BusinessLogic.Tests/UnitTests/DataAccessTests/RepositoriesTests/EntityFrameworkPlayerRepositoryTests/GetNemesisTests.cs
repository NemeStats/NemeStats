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

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetNemesisTests
    {
        private EntityFrameworkPlayerRepository playerRepositoryPartialMock;
        private DataContext dataContextMock;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            playerRepositoryPartialMock = MockRepository.GeneratePartialMock<EntityFrameworkPlayerRepository>(dataContextMock);
            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = 1881
            };
        }

        [Test]
        public void ItThrowsAnUnauthorizedExceptionIfTheUserDoesntHaveAccessToThePlayer()
        {
            int playerId = 123;
            playerRepositoryPartialMock.Expect(partialMock => partialMock.GetPlayer(playerId, currentUser))
                .Throw(new UnauthorizedAccessException());

            Assert.Throws<UnauthorizedAccessException>(() => playerRepositoryPartialMock.GetNemesis(playerId, currentUser));
        }
    }
}
