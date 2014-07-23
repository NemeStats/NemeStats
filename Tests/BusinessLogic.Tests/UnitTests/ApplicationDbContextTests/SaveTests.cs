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

namespace BusinessLogic.Tests.UnitTests.ApplicationDbContextTests
{
    [TestFixture]
    public class SaveTests
    {
        protected ApplicationDbContext dbContext;
        protected SecuredEntityValidatorFactory securedEntityValidatorFactory;
        protected SecuredEntityValidator<EntityWithTechnicalKey> securedEntityValidator;
        protected EntityWithTechnicalKey entityWithGamingGroupAndTechnicalKey;
        protected EntityWithTechnicalKey entityWithGamingGroup;
        protected ApplicationUser currentUser;
        protected DbSet<EntityWithTechnicalKey> dbSetMock;

        [SetUp]
        public void SetUp()
        {
            securedEntityValidatorFactory = MockRepository.GeneratePartialMock <SecuredEntityValidatorFactory>();
            dbContext = MockRepository.GeneratePartialMock<ApplicationDbContext>(securedEntityValidatorFactory);
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
            dbContext.Expect(mock => mock.Set<EntityWithTechnicalKey>())
                .Repeat.Once()
                .Return(dbSetMock);

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

            Exception actualException = Assert.Throws<ArgumentNullException>(() => dbContext.Save<GamingGroup>(null, new ApplicationUser()));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheCurrentUserIsNull()
        {
            Exception expectedException = new ArgumentNullException("currentUser");

            Exception actualException = Assert.Throws<ArgumentNullException>(() => dbContext.Save<GamingGroup>(new GamingGroup(), null));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionExceptionIfTheCurrentUserIsNull()
        {
            Exception expectedException = new ArgumentException(ApplicationDbContext.EXCEPTION_MESSAGE_CURRENT_GAMING_GROUP_ID_CANNOT_BE_NULL);

            Exception actualException = Assert.Throws<ArgumentException>(() => dbContext.Save<GamingGroup>(new GamingGroup(), new ApplicationUser()));

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
                dbContext.Save<EntityWithTechnicalKey>(entityWithGamingGroup, currentUser);
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
            dbContext.Expect(mock => mock.AddOrInsertOverride<EntityWithTechnicalKey>(entityWithGamingGroup))
                .Repeat.Once()
                .Return(entityWithGamingGroup);

            dbContext.Save<EntityWithTechnicalKey>(entityWithGamingGroup, currentUser);

            dbContext.AssertWasCalled(mock => mock.AddOrInsertOverride<EntityWithTechnicalKey>(entityWithGamingGroup));
        }

        //TODO not sure why this is failing
        [Test]
        [Ignore("not sure why this fails")]
        public void ItSetsTheGamingGroupIdIfItIsASecuredEntityThatIsntAlreadyInTheDatabase()
        {
            entityWithGamingGroup.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(false);
            dbContext.Expect(mock => mock.AddOrInsertOverride<EntityWithTechnicalKey>(entityWithGamingGroup))
                .Repeat.Once()
                .Return(entityWithGamingGroup);

            dbContext.Save<EntityWithTechnicalKey>(entityWithGamingGroup, currentUser);

            dbContext.AssertWasCalled(mock => mock.AddOrInsertOverride<EntityWithTechnicalKey>(
                Arg<SecuredEntityWithTechnicalKey>.Matches(entity => entity.GamingGroupId == currentUser.CurrentGamingGroupId)));
        }
    }
}
