using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class DeleteByIdTests : NemeStatsDataContextTestBase
    {
        private SecuredEntityValidator<GamingGroup> securedEntityValidator;
        private DbSet<GamingGroup> gamingGroupDbSetMock;

        [SetUp]
        public void SetUp()
        {
            gamingGroupDbSetMock = MockRepository.GenerateMock<DbSet<GamingGroup>>();

            nemeStatsDbContext.Expect(mock => mock.Set<GamingGroup>())
                .Repeat.Once()
                .Return(gamingGroupDbSetMock);

            securedEntityValidator = MockRepository.GenerateMock<SecuredEntityValidator<GamingGroup>>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<GamingGroup>())
                .Repeat.Once()
                .Return(securedEntityValidator);
        }

        [Test]
        public void ItDeletesTheSpecifiedEntity()
        {
            int id = 1;
            GamingGroup group = new GamingGroup() { Id = id };
            dataContext.Expect(mock => mock.FindById<GamingGroup>(id, currentUser))
                .Return(group);

            dataContext.DeleteById<GamingGroup>(id, currentUser);
            dataContext.CommitAllChanges();

            gamingGroupDbSetMock.AssertWasCalled(mock => mock.Remove(group));
        }
    }
}
