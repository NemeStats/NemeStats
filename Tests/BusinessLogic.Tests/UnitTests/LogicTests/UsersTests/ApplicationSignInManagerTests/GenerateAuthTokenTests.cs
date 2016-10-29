using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
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
        private string _expectedSalt = "some salt";
        private ApplicationUser _applicationUser;
        const string _expectedSaltedHashedAuthToken = "some salted hashed auth token";
        private string _expectedAuthToken = "some auth token";
        private const string ApplicationUserId = "some application user id";
        private UserDeviceAuthToken _userDeviceAuthTokenWithNoDeviceId;
        private UserDeviceAuthToken _userDeviceAuthTokenThatDoesntExpire;
        private UserDeviceAuthToken _userDeviceAuthTokenThatExpiresInThePast;
        private UserDeviceAuthToken _userDeviceAuthTokenThatExpiresInTheFuture;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<AuthTokenGenerator>();
            _autoMocker.PartialMockTheClassUnderTest();

            IAppSettings appSettingsMock = MockRepository.GenerateMock<IAppSettings>();
            appSettingsMock.Expect(mock => mock[AuthTokenGenerator.APP_KEY_AUTH_TOKEN_SALT]).Return(_expectedSalt);

            _autoMocker.Get<IConfigurationManager>().Expect(mock => mock.AppSettings).Return(appSettingsMock);
            _autoMocker.ClassUnderTest.Expect(mock => mock.GenerateNewAuthToken()).Return(_expectedAuthToken);
            _autoMocker.ClassUnderTest.Expect(mock => mock.HashAuthToken(_expectedAuthToken))
                      .Return(_expectedSaltedHashedAuthToken);

            _applicationUser = new ApplicationUser
            {
                Id = ApplicationUserId
            };

            _autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<ApplicationUser>(Arg<string>.Is.Anything)).Return(_applicationUser);

            _userDeviceAuthTokenWithNoDeviceId = new UserDeviceAuthToken
            {
                Id = 0,
                ApplicationUserId = ApplicationUserId,
                DeviceId = null
            };
            _userDeviceAuthTokenThatDoesntExpire = new UserDeviceAuthToken
            {
                Id = 1,
                ApplicationUserId = ApplicationUserId
            };
            _userDeviceAuthTokenThatExpiresInTheFuture = new UserDeviceAuthToken
            {
                Id = 2,
                ApplicationUserId = ApplicationUserId,
                DeviceId = "device id for future expiration",
                AuthenticationTokenExpirationDate = DateTime.UtcNow.AddDays(1)
            };
            _userDeviceAuthTokenThatExpiresInThePast = new UserDeviceAuthToken
            {
                Id = 3,
                ApplicationUserId = ApplicationUserId,
                DeviceId = "device id for already expired",
                AuthenticationTokenExpirationDate = DateTime.UtcNow.AddDays(-1)
            };
            var authTokens = new List<UserDeviceAuthToken>
            {
                _userDeviceAuthTokenWithNoDeviceId,
                _userDeviceAuthTokenThatDoesntExpire,
                _userDeviceAuthTokenThatExpiresInTheFuture,
                _userDeviceAuthTokenThatExpiresInThePast,
                new UserDeviceAuthToken
                {
                    ApplicationUserId = "some other applicationUserId"
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserDeviceAuthToken>()).Return(authTokens);
        }

        [Test]
        public void It_Updates_The_Users_Existing_Auth_Token_With_A_Salted_And_Hashed_Token_When_There_Is_No_Device_Id_Specified()
        {
            //--act
            _autoMocker.ClassUnderTest.GenerateAuthToken(ApplicationUserId);

            //--assert
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(Arg<UserDeviceAuthToken>.Matches(x => x.ApplicationUserId == ApplicationUserId
                                                                                     && x.AuthenticationToken == _expectedSaltedHashedAuthToken
                                                                                     && x.Id == _userDeviceAuthTokenWithNoDeviceId.Id),
                                                                             Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Updates_The_Users_Existing_Auth_Token_With_A_Salted_And_Hashed_Token_When_There_Is_A_Device_Id_Specified()
        {
            //--act
            _autoMocker.ClassUnderTest.GenerateAuthToken(ApplicationUserId, _userDeviceAuthTokenThatExpiresInTheFuture.DeviceId);

            //--assert
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(Arg<UserDeviceAuthToken>.Matches(x => x.ApplicationUserId == ApplicationUserId
                                                                                     && x.AuthenticationToken == _expectedSaltedHashedAuthToken
                                                                                     && x.DeviceId == _userDeviceAuthTokenThatExpiresInTheFuture.DeviceId
                                                                                     && x.Id == _userDeviceAuthTokenThatExpiresInTheFuture.Id),
                                                                             Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Creates_A_New_UserDeviceAuthToken_If_One_Doesnt_Exist_For_The_Specified_User_Id_And_Device_Id()
        {
            //--act
            string deviceId = "some unique device id";
            _autoMocker.ClassUnderTest.GenerateAuthToken(ApplicationUserId, deviceId);

            //--assert
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(Arg<UserDeviceAuthToken>.Matches(x => x.ApplicationUserId == ApplicationUserId
                                                                                     && x.AuthenticationToken == _expectedSaltedHashedAuthToken
                                                                                     && x.DeviceId == deviceId),
                                                                             Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Sets_The_Authentication_Token_Expiration_To_Three_Months_From_Now()
        {
            var result = _autoMocker.ClassUnderTest.GenerateAuthToken("some user id");

            var threeMonthsFromNow = DateTime.UtcNow.AddMonths(3).Date;
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(Arg<UserDeviceAuthToken>.Matches(x => x.AuthenticationTokenExpirationDate.Date == threeMonthsFromNow),
                                                                             Arg<ApplicationUser>.Is.Anything));
            Assert.That(result.AuthenticationTokenExpirationDateTime.Date, Is.EqualTo(threeMonthsFromNow));
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
