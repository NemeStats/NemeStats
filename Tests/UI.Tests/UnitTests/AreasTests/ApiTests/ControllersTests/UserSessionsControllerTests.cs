using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Shouldly;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.Transformations;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    [TestFixture]
    public class UserSessionsControllerTests
    {
        private RhinoAutoMocker<UserSessionsController> autoMocker;
        private IUserStore<ApplicationUser> userStoreMock;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<UserSessionsController>();

            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            var dataProtectionProvider = MockRepository.GenerateMock<IDataProtectionProvider>();
            dataProtectionProvider.Expect(mock => mock.Create("ASP.N‌​ET Identity")).Return(dataProtector);
            var applicationUserManager = MockRepository.GeneratePartialMock<ApplicationUserManager>(userStoreMock, dataProtectionProvider);
            autoMocker.Inject(applicationUserManager);

            var controllerContextMock = MockRepository.GeneratePartialMock<HttpControllerContext>();
            var actionDescriptorMock = MockRepository.GenerateMock<HttpActionDescriptor>();
            autoMocker.ClassUnderTest.ActionContext = new HttpActionContext(controllerContextMock, actionDescriptorMock);
            autoMocker.ClassUnderTest.Request = new HttpRequestMessage();
            autoMocker.ClassUnderTest.Request.SetConfiguration(new HttpConfiguration());

            AutomapperConfiguration.Configure();
        }

        [Test]
        public async Task ItReturnsAnHttp401NotAuthorizedResponseIfTheUsernameAndPasswordIsNotValid()
        {
            var credentialsMessage = new CredentialsMessage
            {
                UserName = "invalid username",
                Password = "invalid password"
            };

            autoMocker.Get<ApplicationUserManager>().Expect(mock => mock.FindAsync(
                                        Arg<string>.Matches(userName => userName == credentialsMessage.UserName),
                                        Arg<string>.Matches(password => password == credentialsMessage.Password)))
                    .Return(Task.FromResult((ApplicationUser)null));

            var actualResponse = await autoMocker.ClassUnderTest.Login(credentialsMessage);

            AssertThatApiAction.HasThisError(actualResponse, HttpStatusCode.Unauthorized, "Invalid credentials provided.");
        }

        [Test]
        public async Task ItSavesAndReturnsANewAuthTokenIfLoginWasSuccessful()
        {
            var credentialsMessage = new CredentialsMessage
            {
                UserName = "valid username",
                Password = "valid corresponding password"
            };

            var loggedInUser = new ApplicationUser
            {
                Id = "some user id",
                AuthenticationTokenExpirationDate = new DateTime()
            };

            autoMocker.Get<ApplicationUserManager>().Expect(mock => mock.FindAsync(
                                        Arg<string>.Matches(userName => userName == credentialsMessage.UserName),
                                        Arg<string>.Matches(password => password == credentialsMessage.Password)))
                    .Return(Task.FromResult(loggedInUser));

            var someNewAuthToken = new AuthToken("the auth token value", new DateTime());
            autoMocker.Get<IAuthTokenGenerator>().Expect(mock => mock.GenerateAuthToken(loggedInUser.Id)).Return(someNewAuthToken);

            var actualResponse = await autoMocker.ClassUnderTest.Login(credentialsMessage);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewAuthTokenMessage>)));
            var content = actualResponse.Content as ObjectContent<NewAuthTokenMessage>;
            var newAuthTokenMessage = content.Value as NewAuthTokenMessage;
            Assert.That(newAuthTokenMessage.AuthenticationToken, Is.EqualTo(someNewAuthToken.AuthenticationTokenString));
            Assert.That(newAuthTokenMessage.AuthenticationTokenExpirationDateTime, Is.EqualTo(loggedInUser.AuthenticationTokenExpirationDate));
        }

        [Test]
        public async Task ItUsesTheExistingAuthenticationTokenIfSpecified()
        {
            var credentialsMessage = new CredentialsMessage
            {
                UserName = "valid username",
                Password = "valid corresponding password",
                PreserveExistingAuthenticationToken = true
            };

            var loggedInUser = new ApplicationUser
            {
                Id = "some user id",
                AuthenticationTokenExpirationDate = new DateTime(),
                AuthenticationToken = "some existinig auth token"
            };

            autoMocker.Get<ApplicationUserManager>().Expect(mock => mock.FindAsync(
                                        Arg<string>.Matches(userName => userName == credentialsMessage.UserName),
                                        Arg<string>.Matches(password => password == credentialsMessage.Password)))
                    .Return(Task.FromResult(loggedInUser));


            var actualResponse = await autoMocker.ClassUnderTest.Login(credentialsMessage);

            autoMocker.Get<IAuthTokenGenerator>().AssertWasNotCalled(mock => mock.GenerateAuthToken(Arg<string>.Is.Anything));
            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewAuthTokenMessage>)));
            var content = actualResponse.Content as ObjectContent<NewAuthTokenMessage>;
            var newAuthTokenMessage = content.Value as NewAuthTokenMessage;
            newAuthTokenMessage.AuthenticationToken.ShouldBe(loggedInUser.AuthenticationToken);
        }
    }
}
