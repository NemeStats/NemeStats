using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests
{
    [TestFixture]
    public class SaveTests : EntityFrameworkGameDefinitionRepositoryTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            gameDefinitionsDbSetMock.Expect(mock => mock.Add(gameDefinition));
        }

        [Test]
        public void ItAddsTheGameDefinitionIfItIsntAlreadyInTheDatabase()
        {
            gameDefinitionRepositoryPartialMock.Save(gameDefinition, currentUser);

            gameDefinitionsDbSetMock.AssertWasCalled(mock => mock.Add(gameDefinition));
            dbContextMock.AssertWasCalled(mock => mock.SaveChanges());
        }

        //TODO having trouble mocking dbEntityEntry. One more reason I need to implement a real repository pattern.
        /*
        [Test]
        public void ItMarksTheEntityAsModifiedIfItIsAlreadyInTheDatabase()
        {
            DbEntityEntry<GameDefinition> dbEntityEntry = MockRepository.GenerateMock<DbEntityEntry<GameDefinition>>();
            dbContextMock.Expect(mock => mock.Entry(gameDefinition))
                .Repeat.Once()
                .Return(dbEntityEntry);
            dbEntityEntry.Expect(mock => mock.State = EntityState.Modified);

            gameDefinitionRepositoryPartialMock.Save(gameDefinition, currentUser);

            dbEntityEntry.VerifyAllExpectations();
            dbContextMock.AssertWasCalled(mock => mock.SaveChanges());
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheGameDefinitionIsExistingAndTheUserGamingGroupDoesNotMatch()
        {
            GameDefinition mismatchedGameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            mismatchedGameDefinition.GamingGroupId = -1;
            mismatchedGameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            gameDefinitionRepositoryPartialMock.Expect(mock => mock.ValidateUserHasAccessToGameDefinition(currentUser, mismatchedGameDefinition));
            DbEntityEntry<GameDefinition> dbEntityEntry = MockRepository.GeneratePartialMock<DbEntityEntry<GameDefinition>>();
            dbContextMock.Expect(mock => mock.Entry(mismatchedGameDefinition))
                .Repeat.Once()
                .Return(dbEntityEntry);

            gameDefinitionRepositoryPartialMock.Save(mismatchedGameDefinition, currentUser);

            gameDefinitionRepositoryPartialMock.AssertWasCalled(
                mock => mock.ValidateUserHasAccessToGameDefinition(currentUser, mismatchedGameDefinition));
        }*/

        [Test]
        public void ItSetsTheGamingGroupIdToThatOfTheCurrentUser()
        {
            gameDefinitionRepositoryPartialMock.Save(gameDefinition, currentUser);

            gameDefinitionsDbSetMock.AssertWasCalled(mock => mock.Add(
                Arg<GameDefinition>.Matches(
                    gameDef => gameDef.GamingGroupId == currentUser.CurrentGamingGroupId)));
        }
    }
}
