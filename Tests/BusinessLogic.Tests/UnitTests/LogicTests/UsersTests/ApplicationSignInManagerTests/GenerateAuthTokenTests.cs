using System;
using System.Configuration.Abstractions;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.ApplicationSignInManagerTests
{
    [TestFixture]
    public class GenerateAuthTokenTests
    {
        private RhinoAutoMocker<AuthTokenGenerator> _autoMocker;
        private const string ExpectedSalt = "some salt";
        private ApplicationUser _applicationUser;
        private const string ApplicationUserId = "some application user id";

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<AuthTokenGenerator>();

            IAppSettings appSettingsMock = MockRepository.GenerateMock<IAppSettings>();
            appSettingsMock.Expect(mock => mock[AuthTokenGenerator.APP_KEY_AUTH_TOKEN_SALT]).Return(ExpectedSalt);
            _autoMocker.Get<IConfigurationManager>().Expect(mock => mock.AppSettings).Return(appSettingsMock);

            _applicationUser = new ApplicationUser
            {
                Id = ApplicationUserId
            };

            _autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<ApplicationUser>(Arg<string>.Is.Anything)).Return(_applicationUser);
        }

        [Test]
        public void ItUpdatesTheAspNetUsersAuthenticationTokenWithAHashedAndSaltedToken()
        {
            _autoMocker.PartialMockTheClassUnderTest();
            const string expectedAuthToken = "some auth token";
            
            _autoMocker.ClassUnderTest.Expect(mock => mock.GenerateNewAuthToken()).Return(expectedAuthToken);
            const string expectedSaltedHashedAuthToken = "some salted hashed auth token";
            _autoMocker.ClassUnderTest.Expect(mock => mock.HashAuthToken(expectedAuthToken))
                      .Return(expectedSaltedHashedAuthToken);

            _autoMocker.ClassUnderTest.GenerateAuthToken(ApplicationUserId);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(Arg<ApplicationUser>.Matches(user => user.Id == ApplicationUserId
                                                                                     && user.AuthenticationToken == expectedSaltedHashedAuthToken),
                                                                             Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheAuthenticationTokenExpirationForThreeMonthsFromNow()
        {
            var result = _autoMocker.ClassUnderTest.GenerateAuthToken("some user id");

            var threeMonthsFromNow = DateTime.UtcNow.AddMonths(3).Date;
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(Arg<ApplicationUser>.Matches(user => user.AuthenticationTokenExpirationDate.Value.Date == threeMonthsFromNow),
                                                                             Arg<ApplicationUser>.Is.Anything));
            Assert.That(result.AuthenticationTokenExpirationDateTime.HasValue);
            Assert.That(result.AuthenticationTokenExpirationDateTime.Value.Date, Is.EqualTo(threeMonthsFromNow));
        }

        [Test]
        public void ItReturnsTheUnsaltedUnhashedAuthToken()
        {
            _autoMocker.PartialMockTheClassUnderTest();
            var expectedAuthToken = "some token";
            _autoMocker.ClassUnderTest.Expect(mock => mock.GenerateNewAuthToken()).Return(expectedAuthToken);

            var actualAuthToken = _autoMocker.ClassUnderTest.GenerateAuthToken("some id");

            Assert.That(actualAuthToken.AuthenticationTokenString, Is.EqualTo(expectedAuthToken));
        }
    }
}
