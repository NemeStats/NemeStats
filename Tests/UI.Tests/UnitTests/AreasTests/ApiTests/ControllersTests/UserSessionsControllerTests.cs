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
using UI.Areas.Api.Controllers;
using UI.Models.API;
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

            IDataProtector dataProtector = MockRepository.GenerateMock<IDataProtector>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            IDataProtectionProvider dataProtectionProvider = MockRepository.GenerateMock<IDataProtectionProvider>();
            dataProtectionProvider.Expect(mock => mock.Create("ASP.N‌​ET Identity")).Return(dataProtector);
            ApplicationUserManager applicationUserManager = MockRepository.GeneratePartialMock<ApplicationUserManager>(userStoreMock, dataProtectionProvider);
            autoMocker.Inject(applicationUserManager);

            var controllerContextMock = MockRepository.GeneratePartialMock<HttpControllerContext>();
            HttpActionDescriptor actionDescriptorMock = MockRepository.GenerateMock<HttpActionDescriptor>();
            autoMocker.ClassUnderTest.ActionContext = new HttpActionContext(controllerContextMock, actionDescriptorMock);
<<<<<<< HEAD

=======
>>>>>>> origin
            autoMocker.ClassUnderTest.Request = new HttpRequestMessage();
            autoMocker.ClassUnderTest.Request.SetConfiguration(new HttpConfiguration());

            AutomapperConfiguration.Configure();
        }

        [Test]
        public async Task ItReturnsAnHttp401NotAuthorizedResponseIfTheUsernameAndPasswordIsNotValid()
        {
            CredentialsMessage credentialsMessage = new CredentialsMessage
            {
                UserName = "invalid username",
                Password = "invalid password"
            };

            autoMocker.Get<ApplicationUserManager>().Expect(mock => mock.FindAsync(
                                        Arg<string>.Matches(userName => userName == credentialsMessage.UserName),
                                        Arg<string>.Matches(password => password == credentialsMessage.Password)))
                    .Return(Task.FromResult((ApplicationUser)null));

            HttpResponseMessage actualResponse = await autoMocker.ClassUnderTest.Login(credentialsMessage);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = actualResponse.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(actualResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo("Invalid credentials provided."));
        }

        [Test]
        public async Task ItSavesAndReturnsANewAuthTokenIfLoginWasSuccessful()
        {
            CredentialsMessage credentialsMessage = new CredentialsMessage
            {
                UserName = "valid username",
                Password = "valid corresponding password"
            };

            ApplicationUser loggedInUser = new ApplicationUser
            {
                Id = "some user id"
            };

            autoMocker.Get<ApplicationUserManager>().Expect(mock => mock.FindAsync(
                                        Arg<string>.Matches(userName => userName == credentialsMessage.UserName),
                                        Arg<string>.Matches(password => password == credentialsMessage.Password)))
                    .Return(Task.FromResult(loggedInUser));

            String someNewAuthToken = "the auth token value";
            autoMocker.Get<IAuthTokenGenerator>().Expect(mock => mock.GenerateAuthToken(loggedInUser.Id)).Return(someNewAuthToken);

            HttpResponseMessage actualResponse = await autoMocker.ClassUnderTest.Login(credentialsMessage);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewAuthTokenMessage>)));
            ObjectContent<NewAuthTokenMessage> content = actualResponse.Content as ObjectContent<NewAuthTokenMessage>;
            NewAuthTokenMessage newAuthTokenMessage = content.Value as NewAuthTokenMessage;
            Assert.That(newAuthTokenMessage.AuthenticationToken, Is.EqualTo(someNewAuthToken));
        }
    }
}
