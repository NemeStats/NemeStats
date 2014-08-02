using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests
{
    [TestFixture]
    public class DeleteTests : EntityFrameworkGameDefinitionRepositoryTestBase
    {
        [Test]
        public void ItRetrievesTheGameDefinitionInASecureManner()
        {
            gameDefinitionRepositoryPartialMock.Expect(mock => mock.GetGameDefinition(gameDefinition.Id, currentUser))
                .Repeat.Once()
                .Return(gameDefinition);

            gameDefinitionRepositoryPartialMock.Delete(gameDefinition.Id, currentUser);

            //TODO calling GetGameDefinition implies that authorization validation will occur. Does this test make sense?
            gameDefinitionRepositoryPartialMock.VerifyAllExpectations();
        }

        [Test]
        public void ItDeletesTheEntity()
        {
            gameDefinitionRepositoryPartialMock.Expect(mock => mock.GetGameDefinition(gameDefinition.Id, currentUser))
             .Repeat.Once()
             .Return(gameDefinition);
            dataContextMock.Expect(mock => mock.Delete<GameDefinition>(gameDefinition, currentUser));

            gameDefinitionRepositoryPartialMock.Delete(gameDefinition.Id, currentUser);

            dataContextMock.VerifyAllExpectations();
        }
    }
}
