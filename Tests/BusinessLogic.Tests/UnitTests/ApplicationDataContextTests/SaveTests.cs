using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
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

namespace BusinessLogic.Tests.UnitTests.ApplicationDataContextTests
{
    [TestFixture]
    public class SaveTests
    {
        protected ApplicationDataContext dataContext;
        protected SecuredEntityValidatorFactory securedEntityValidatorFactory;
        protected SecuredEntityValidator<EntityWithTechnicalKey> securedEntityValidator;
        protected EntityWithTechnicalKey entityWithGamingGroupAndTechnicalKey;
        protected EntityWithTechnicalKey entityWithGamingGroup;
        protected ApplicationUser currentUser;
        protected DbSet<EntityWithTechnicalKey> dbSetMock;

        [SetUp]
        public void SetUp()
        {
            NemeStatsDbContext nemeStatsDbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            securedEntityValidatorFactory = MockRepository.GeneratePartialMock <SecuredEntityValidatorFactory>();
            dataContext = MockRepository.GeneratePartialMock<ApplicationDataContext>(nemeStatsDbContext, securedEntityValidatorFactory);
            securedEntityValidator = MockRepository.GenerateMock<SecuredEntityValidator<EntityWithTechnicalKey>>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<EntityWithTechnicalKey>())
                .Repeat.Once()
                .Return(securedEntityValidator);
            entityWithGamingGroupAndTechnicalKey = MockRepository.GenerateStub<EntityWithTechnicalKey>();
            entityWithGamingGroupAndTechnicalKey.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            entityWithGamingGroup = MockRepository.GenerateStub<EntityWithTechnicalKey>();
            dbSetMock = MockRepository.GenerateMock<DbSet<EntityWithTechnicalKey>>();

            currentUser = new ApplicationUser()
            {
                Id = "application user id",
                CurrentGamingGroupId = 1
            };
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheEntityIsNull()
        {
            Exception expectedException = new ArgumentNullException("entity");

            Exception actualException = Assert.Throws<ArgumentNullException>(() => dataContext.Save<GamingGroup>(null, new ApplicationUser()));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheCurrentUserIsNull()
        {
            Exception expectedException = new ArgumentNullException("currentUser");

            Exception actualException = Assert.Throws<ArgumentNullException>(() => dataContext.Save<GamingGroup>(new GamingGroup(), null));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheCurrentUsersGamingGroupIsNullAndTheEntityIsSecured()
        {
            dataContext.Expect(mock => mock.AddOrInsertOverride(Arg<GameDefinition>.Is.Anything));
            Exception expectedException = new ArgumentException(ApplicationDataContext.EXCEPTION_MESSAGE_CURRENT_GAMING_GROUP_ID_CANNOT_BE_NULL);

            Exception actualException = Assert.Throws<ArgumentException>(() => dataContext.Save<GameDefinition>(new GameDefinition(), new ApplicationUser()));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItValidatesSecurityIfTheEntityIsAlreadyInTheDatabase()
        {
            entityWithGamingGroup.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            securedEntityValidator.Expect(mock => mock.ValidateAccess(entityWithGamingGroup, currentUser, typeof(EntityWithTechnicalKey)))
                .Throw(new UnauthorizedAccessException());
            try
            {
                dataContext.Save<EntityWithTechnicalKey>(entityWithGamingGroup, currentUser);
            }
            catch (UnauthorizedAccessException) { }

            securedEntityValidator.VerifyAllExpectations();
        }

        [Test]
        public void ItAddsOrSavesTheEntity()
        {
            entityWithGamingGroup.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            dataContext.Expect(mock => mock.AddOrInsertOverride<EntityWithTechnicalKey>(entityWithGamingGroup))
                .Repeat.Once()
                .Return(entityWithGamingGroup);

            dataContext.Save<EntityWithTechnicalKey>(entityWithGamingGroup, currentUser);

            dataContext.AssertWasCalled(mock => mock.AddOrInsertOverride<EntityWithTechnicalKey>(entityWithGamingGroup));
        }

        [Test]
        public void ItSetsTheGamingGroupIdIfItIsASecuredEntityThatIsntAlreadyInTheDatabase()
        {
            GameDefinition gameDefinition = new GameDefinition();

            dataContext.Expect(mock => mock.AddOrInsertOverride(gameDefinition))
                .Repeat.Once()
                .Return(gameDefinition);

            dataContext.Save(gameDefinition, currentUser);

            dataContext.AssertWasCalled(mock => mock.AddOrInsertOverride(
                Arg<GameDefinition>.Matches(entity => entity.GamingGroupId == currentUser.CurrentGamingGroupId)));
        }
    }
}
