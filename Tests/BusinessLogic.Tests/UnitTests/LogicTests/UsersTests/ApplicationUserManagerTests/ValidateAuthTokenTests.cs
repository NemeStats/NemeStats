using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
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
        private UserDeviceAuthToken _expectedUserDeviceAuthTokenThatIsntExpired;
        private string expiredAuthToken = "some expired auth token";
        private UserDeviceAuthToken _expectedUserDeviceAuthTokenThatIsExpired;
        
        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<AuthTokenValidator>();

            const string EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN = "some hashed and salted auth token";
            autoMocker.Get<IAuthTokenGenerator>().Expect(mock => mock.HashAuthToken(this.validAuthToken)).Return(
                EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN);

            _expectedUserDeviceAuthTokenThatIsntExpired = new UserDeviceAuthToken()
            {
                AuthenticationToken = EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN,
                AuthenticationTokenExpirationDate = DateTime.UtcNow.AddDays(3)
            };

            const string EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN_THAT_IS_EXPIRED = "some hashed and salted auth token that is expired";
            autoMocker.Get<IAuthTokenGenerator>().Expect(mock => mock.HashAuthToken(this.expiredAuthToken)).Return(
                EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN_THAT_IS_EXPIRED);

            _expectedUserDeviceAuthTokenThatIsExpired = new UserDeviceAuthToken()
            {
                AuthenticationToken = EXPECTED_HASHED_AND_SALTED_AUTH_TOKEN,
                AuthenticationTokenExpirationDate = DateTime.UtcNow.AddDays(-1)
            };
            IQueryable<UserDeviceAuthToken> userDeviceAuthTokens = new List<UserDeviceAuthToken>
            {
                _expectedUserDeviceAuthTokenThatIsntExpired,
                _expectedUserDeviceAuthTokenThatIsExpired
            }.AsQueryable();

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserDeviceAuthToken>()).Return(userDeviceAuthTokens);
        }

        [Test]
        public void ItReturnsTheUserDeviceAuthTokenIfTheSaltedHashedTokenMatchesAndTheTokenIsNotExpired()
        {
            var result = autoMocker.ClassUnderTest.ValidateAuthToken(this.validAuthToken);

            Assert.That(result, Is.EqualTo(this._expectedUserDeviceAuthTokenThatIsntExpired));
        }

        [Test]
        public void ItReturnsNullIfTheAuthTokenDoesntExist()
        {
            var result = autoMocker.ClassUnderTest.ValidateAuthToken("some non-existent hash token");

            Assert.That(result, Is.EqualTo(null));
        }

        [Test]
        public void ItReturnsNullIfTheAuthTokenIsExpired()
        {
            var result = autoMocker.ClassUnderTest.ValidateAuthToken(expiredAuthToken);

            Assert.That(result, Is.EqualTo(null));
        }
    }
}
