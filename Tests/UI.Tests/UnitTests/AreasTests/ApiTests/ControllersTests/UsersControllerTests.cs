using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.Transformations;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    [TestFixture]
    public class UsersControllerTests : ApiControllerTestBase<UsersController>
    {
        private RhinoAutoMocker<UsersController> autoMocker;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<UsersController>();
            autoMocker.ClassUnderTest.Request = new HttpRequestMessage();
            autoMocker.ClassUnderTest.Request.SetConfiguration(new HttpConfiguration());

            AutomapperConfiguration.Configure();
        }

        [Test]
        public async Task Post_RegistersTheGivenUser()
        {
            autoMocker.Get<IUserRegisterer>().Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything)).Return(Task.FromResult(new RegisterNewUserResult{ Result = IdentityResult.Failed("some failure")}));

            var newUserMessage = new NewUserMessage
            {
                EmailAddress = "email@email.com",
                UserName = "username",
                Password = "password"
            };

            await autoMocker.ClassUnderTest.RegisterNewUser(newUserMessage);

            autoMocker.Get<IUserRegisterer>().AssertWasCalled(mock => mock.RegisterUser(
                Arg<NewUser>.Matches(user =>  user.EmailAddress == newUserMessage.EmailAddress 
                    && user.UserName == newUserMessage.UserName
                    && user.Password == newUserMessage.Password)));
        }

        [Test]
        public async Task Post_ReturnsAnHttp400BadRequestResponseWhenThereIsAnIssueRegistering()
        {
            var registerNewUserResult = new RegisterNewUserResult
            {
                Result = IdentityResult.Failed("some error")
            };
            autoMocker.Get<IUserRegisterer>().Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything)).Return(Task.FromResult(registerNewUserResult));

            var newUserMessage = new NewUserMessage
            {
                EmailAddress = "email@email.com",
                UserName = "username",
                Password = "password"
            };

            var actualResponse = await autoMocker.ClassUnderTest.RegisterNewUser(newUserMessage);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = actualResponse.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(actualResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(httpError.Message, Is.EqualTo(registerNewUserResult.Result.Errors.First()));
        }

        [Test]
        public async Task Post_ReturnsANewlyRegisteredUserMessage()
        {
            var newUserMessage = new NewUserMessage();
            var expectedNewlyRegisteredUserMessage = new NewlyRegisteredUserMessage
            {
                UserId = "user id",
                PlayerId = 2,
                PlayerName = "player name",
                GamingGroupId = 3,
                GamingGroupName = "gaming group name"
            };
            var registerNewUserResult = new RegisterNewUserResult
            {
                Result = IdentityResult.Success,
                NewlyRegisteredUser = new NewlyRegisteredUser
                {
                    UserId = "user id",
                    PlayerId = 2,
                    PlayerName = "player name",
                    GamingGroupId = 3,
                    GamingGroupName = "gaming group name"
                }
            };
            autoMocker.Get<IUserRegisterer>().Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything)).Return(Task.FromResult(registerNewUserResult));
            var expectedAuthToken = "auth token";
            autoMocker.Get<IAuthTokenGenerator>().Expect(mock => mock.GenerateAuthToken(expectedNewlyRegisteredUserMessage.UserId)).Return(expectedAuthToken);

            var actualResponse = await autoMocker.ClassUnderTest.RegisterNewUser(newUserMessage);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewlyRegisteredUserMessage>)));
            var content = actualResponse.Content as ObjectContent<NewlyRegisteredUserMessage>;
            var actualNewlyRegisteredUserMessage = content.Value as NewlyRegisteredUserMessage;
            Assert.That(actualNewlyRegisteredUserMessage.UserId, Is.EqualTo(expectedNewlyRegisteredUserMessage.UserId));
            Assert.That(actualNewlyRegisteredUserMessage.PlayerId, Is.EqualTo(expectedNewlyRegisteredUserMessage.PlayerId));
            Assert.That(actualNewlyRegisteredUserMessage.PlayerName, Is.EqualTo(expectedNewlyRegisteredUserMessage.PlayerName));
            Assert.That(actualNewlyRegisteredUserMessage.GamingGroupId, Is.EqualTo(expectedNewlyRegisteredUserMessage.GamingGroupId));
            Assert.That(actualNewlyRegisteredUserMessage.GamingGroupName, Is.EqualTo(expectedNewlyRegisteredUserMessage.GamingGroupName));
            Assert.That(actualNewlyRegisteredUserMessage.AuthenticationToken, Is.EqualTo(expectedAuthToken));
        }

        [Test]
        public void Get_Returns_User_Information_For_The_Given_User()
        {
            var EXPECTED_USER_ID = "some guid";
            var expectedUserInformation = new UserInformation();
            autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(EXPECTED_USER_ID, applicationUser))
                      .Return(expectedUserInformation);
            UserInformationMessage expectedUserInformationMessage = new UserInformationMessage();
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<UserInformation, UserInformationMessage>(expectedUserInformation))
                .Return(expectedUserInformationMessage);
            autoMocker.ClassUnderTest.CurrentUser = applicationUser;

            var actualResponse = autoMocker.ClassUnderTest.GetUserInformation(EXPECTED_USER_ID);
            
            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<UserInformationMessage>)));
            var content = actualResponse.Content as ObjectContent<UserInformationMessage>;
            var actualUserInformationMessage = content.Value as UserInformationMessage;
            Assert.That(actualUserInformationMessage, Is.SameAs(expectedUserInformationMessage));
        }
    }
}
