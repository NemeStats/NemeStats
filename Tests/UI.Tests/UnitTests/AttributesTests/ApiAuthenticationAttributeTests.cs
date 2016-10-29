using System;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using BusinessLogic.Models;
using UI.Areas.Api;
using UI.Attributes;
using Is = NUnit.Framework.Is;

namespace UI.Tests.UnitTests.AttributesTests
{
    [TestFixture]
    public class ApiAuthenticationAttributeTests
    {
        private ApiAuthenticationAttribute attribute;
        private HttpActionContext actionContext;
        private HttpControllerContext controllerContext;
        private HttpRequestMessage request;
        private IAuthTokenValidator authTokenValidatorMock;
        private ClientIdCalculator clientIdCalculatorMock;

        [SetUp]
        public void SetUp()
        {
            request = new HttpRequestMessage();
            request.SetConfiguration(new HttpConfiguration());
            authTokenValidatorMock = MockRepository.GenerateMock<IAuthTokenValidator>();
            clientIdCalculatorMock = MockRepository.GenerateMock<ClientIdCalculator>();
            attribute = new ApiAuthenticationAttribute(authTokenValidatorMock, clientIdCalculatorMock);
            var controllerMock = MockRepository.GeneratePartialMock<ApiControllerBase>();
            controllerContext = new HttpControllerContext {Request = request, Controller = controllerMock };
            actionContext = new HttpActionContext(controllerContext, new ReflectedHttpActionDescriptor());
        }

        [Test]
        public void ItThrowsAnInvalidOperationExceptionIfTheAttributeIsAppliedToAControllerThatIsntApiControllerBase()
        {
            controllerContext.Controller = MockRepository.GenerateMock<ApiController>();

            var actualException = Assert.Throws<InvalidOperationException>(() => attribute.OnActionExecuting(actionContext));
            Assert.That(actualException.Message, 
                Is.EqualTo("The ApiAuthentication attribute can only be applied to actions in an ApiController that extends ApiControllerBase."));
        }

        [Test]
        public void ShouldReturnBadRequestWhenNoTokenProvided()
        {
            attribute.OnActionExecuting(actionContext);

            Assert.That(actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_MISSING_AUTH_TOKEN_HEADER));
        }

        [Test]
        public void ShouldReturnBadRequestWhenTokenHasEmptyValue()
        {
            request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new [] { string.Empty });
            attribute.OnActionExecuting(actionContext);

            Assert.That(actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ItSetsTheApplicationUserOnTheApiControllerBase()
        {
            const string expectedToken = "TEST";
            request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { expectedToken });
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = 1
            };

            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(expectedToken)).Return(expectedUser);

            attribute.OnActionExecuting(actionContext);
            ApplicationUser actualUser = ((ApiControllerBase)actionContext.ControllerContext.Controller).CurrentUser;
            Assert.That(actualUser, Is.SameAs(expectedUser));
        }

        [Test]
        public void ItSetsTheAnonymousClientIdOnTheApplicationUser()
        {
            const string EXPECTED_TOKEN = "TEST";
            request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { EXPECTED_TOKEN });
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = 1
            };
            var expectedUserDeviceAuthToken = new UserDeviceAuthToken
            {
                ApplicationUser = expectedUser
            };
            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(EXPECTED_TOKEN)).Return(expectedUser);
            const string EXPECTED_CLIENT_ID = "some client id";
            clientIdCalculatorMock.Expect(mock => mock.GetClientId(request, expectedUser)).Return(EXPECTED_CLIENT_ID);

            attribute.OnActionExecuting(actionContext);
            ApplicationUser actualUser = ((ApiControllerBase)actionContext.ControllerContext.Controller).CurrentUser;

            Assert.That(actualUser.AnonymousClientId, Is.EqualTo(EXPECTED_CLIENT_ID));
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenWrongToken()
        {
            const string TOKEN = "DIFFERENT";
            request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { TOKEN });

            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(Arg<string>.Is.Anything)).Return(null);

            attribute.OnActionExecuting(actionContext);

            Assert.That(actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenTokenExpired()
        {
            const string EXPECTED_TOKEN = "TEST";
            request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { EXPECTED_TOKEN });

            attribute.OnActionExecuting(actionContext);

            Assert.That(actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ItReturnsAnUnauthorizedHttpStatusWhenThereIsAGamingGroupIdThatDoesntMatchTheCurrentUser()
        {
            const string TOKEN = "TEST";
            request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { TOKEN });
            const int REQUESTED_GAMING_GROUP_ID = 1;
            actionContext.ActionArguments.Add("gamingGroupId", REQUESTED_GAMING_GROUP_ID);
            const int GAMING_GROUP_ID = -1;
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = GAMING_GROUP_ID
            };
            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(TOKEN)).Return(expectedUser);
            const string EXPECTED_CLIENT_ID = "some client id";
            clientIdCalculatorMock.Expect(mock => mock.GetClientId(request, expectedUser)).Return(EXPECTED_CLIENT_ID);

            attribute.OnActionExecuting(actionContext);

            Assert.That(actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(string.Format(ApiAuthenticationAttribute.ERROR_MESSAGE_UNAUTHORIZED_TO_GAMING_GROUP, REQUESTED_GAMING_GROUP_ID)));
        }
    }
}
