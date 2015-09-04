using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.ApplicationUserManagerTests
{
    [TestFixture]
    public class ValidateAuthTokenTests
    {
        private RhinoAutoMocker<AuthTokenValidator> autoMocker;
        private string validAuthToken = "some auth token";
        private ApplicationUser expectedUserWithValidAuthToken;
        private string expiredAuthToken = "some expired auth token";
        private ApplicationUser expectedUserWithExpiredAuthToken;
        
        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<AuthTokenValidator>();

            const string EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN = "some hashed and salted auth token";
            autoMocker.Get<IAuthTokenGenerator>().Expect(mock => mock.HashAuthToken(this.validAuthToken)).Return(
                EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN);

            this.expectedUserWithValidAuthToken = new ApplicationUser
            {
                AuthenticationToken = EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN,
                AuthenticationTokenExpirationDate = DateTime.UtcNow.AddDays(3)
            };

            const string EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN_THAT_IS_EXPIRED = "some hashed and salted auth token that is expired";
            autoMocker.Get<IAuthTokenGenerator>().Expect(mock => mock.HashAuthToken(this.expiredAuthToken)).Return(
                EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN_THAT_IS_EXPIRED);

            this.expectedUserWithExpiredAuthToken = new ApplicationUser
            {
                AuthenticationToken = EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN,
                AuthenticationTokenExpirationDate = DateTime.UtcNow.AddDays(-1)
            };
            IQueryable<ApplicationUser> applicationUsers = new List<ApplicationUser>
            {
                expectedUserWithValidAuthToken,
                expectedUserWithExpiredAuthToken
            }.AsQueryable();

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(applicationUsers);
        }

        [Test]
        public void ItReturnsTheApplicationUserIfTheSaltedHashedTokenMatchesAndTheTokenIsNotExpired()
        {
            ApplicationUser actualUser = autoMocker.ClassUnderTest.ValidateAuthToken(this.validAuthToken);

            Assert.That(actualUser, Is.EqualTo(this.expectedUserWithValidAuthToken));
        }

        [Test]
        public void ItReturnsNullIfTheAuthTokenDoesntExist()
        {
            ApplicationUser actualUser = autoMocker.ClassUnderTest.ValidateAuthToken("some non-existent hash token");

            Assert.That(actualUser, Is.EqualTo(null));
        }

        [Test]
        public void ItReturnsNullIfTheAuthTokenIsExpired()
        {
            ApplicationUser actualUser = autoMocker.ClassUnderTest.ValidateAuthToken(expiredAuthToken);

            Assert.That(actualUser, Is.EqualTo(null));
        }
    }
}
