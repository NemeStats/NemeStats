using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private RhinoAutoMocker<AuthTokenGenerator> autoMocker;
        private const string expectedSalt = "some salt";
        private ApplicationUser applicationUser;
        private const string applicationUserId = "some application user id";

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<AuthTokenGenerator>();

            IAppSettings appSettingsMock = MockRepository.GenerateMock<IAppSettings>();
            appSettingsMock.Expect(mock => mock[AuthTokenGenerator.APP_KEY_AUTH_TOKEN_SALT]).Return(expectedSalt);
            autoMocker.Get<IConfigurationManager>().Expect(mock => mock.AppSettings).Return(appSettingsMock);

            applicationUser = new ApplicationUser
            {
                Id = applicationUserId
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<ApplicationUser>(Arg<string>.Is.Anything)).Return(applicationUser);
        }

        [Test]
        public void ItUpdatesTheAspNetUsersAuthenticationTokenWithAHashedAndSaltedToken()
        {
            const string applicationUserId = "some user id";
            autoMocker.PartialMockTheClassUnderTest();
            const string expectedAuthToken = "some auth token";
            
            autoMocker.ClassUnderTest.Expect(mock => mock.GenerateNewAuthToken()).Return(expectedAuthToken);
            const string expectedSaltedHashedAuthToken = "some salted hashed auth token";
            autoMocker.ClassUnderTest.Expect(mock => mock.GetNewSaltedHashedAuthenticationToken(expectedSalt, expectedAuthToken))
                      .Return(expectedSaltedHashedAuthToken);

            autoMocker.ClassUnderTest.GenerateAuthToken(applicationUserId);

            autoMocker.Get<IDataContext>().Save(Arg<ApplicationUser>.Matches(user => user.Id == applicationUserId
                                                                                     && user.AuthenticationToken == expectedSaltedHashedAuthToken),
                                                                             Arg<ApplicationUser>.Is.Anything);
        }

        [Test]
        public void ItReturnsTheUnsaltedUnhashedAuthToken()
        {
            autoMocker.PartialMockTheClassUnderTest();
            string expectedAuthToken = "some auth token";
            autoMocker.ClassUnderTest.Expect(mock => mock.GenerateNewAuthToken()).Return(expectedAuthToken);

            string actualAuthToken = autoMocker.ClassUnderTest.GenerateAuthToken("some id");

            Assert.That(actualAuthToken, Is.EqualTo(expectedAuthToken));
        }
    }
}
