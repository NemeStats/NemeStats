using System;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using UI.Areas.Api;
using UI.Attributes;
using Is = NUnit.Framework.Is;

namespace UI.Tests.UnitTests.AttributesTests
{
    [TestFixture]
    public class ApiAuthenticationAttributeTests
    {
        private ApiAuthenticationAttribute _attribute;
        private HttpActionContext _actionContext;
        private HttpControllerContext _controllerContext;
        private HttpRequestMessage _request;
        private IAuthTokenValidator _authTokenValidatorMock;
        private ClientIdCalculator _clientIdCalculatorMock;

        [SetUp]
        public void SetUp()
        {
            _request = new HttpRequestMessage();
            _request.SetConfiguration(new HttpConfiguration());
            _authTokenValidatorMock = MockRepository.GenerateMock<IAuthTokenValidator>();
            _clientIdCalculatorMock = MockRepository.GenerateMock<ClientIdCalculator>();
            _attribute = new ApiAuthenticationAttribute(_authTokenValidatorMock, _clientIdCalculatorMock);
            var controllerMock = MockRepository.GeneratePartialMock<ApiControllerBase>();
            _controllerContext = new HttpControllerContext {Request = _request, Controller = controllerMock };
            _actionContext = new HttpActionContext(_controllerContext, new ReflectedHttpActionDescriptor());
        }

        [Test]
        public void ItThrowsAnInvalidOperationExceptionIfTheAttributeIsAppliedToAControllerThatIsntApiControllerBase()
        {
            _controllerContext.Controller = MockRepository.GenerateMock<ApiController>();

            var actualException = Assert.Throws<InvalidOperationException>(() => _attribute.OnActionExecuting(_actionContext));
            Assert.That(actualException.Message, 
                Is.EqualTo("The ApiAuthentication attribute can only be applied to actions in an ApiController that extends ApiControllerBase."));
        }

        [Test]
        public void ShouldReturnBadRequestWhenNoTokenProvided()
        {
            _attribute.OnActionExecuting(_actionContext);

            Assert.That(_actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = _actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(_actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_MISSING_AUTH_TOKEN_HEADER));
        }

        [Test]
        public void ShouldReturnBadRequestWhenTokenHasEmptyValue()
        {
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new [] { string.Empty });
            _attribute.OnActionExecuting(_actionContext);

            Assert.That(_actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = _actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(_actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ItSetsTheApplicationUserOnTheApiControllerBase()
        {
            const string expectedToken = "TEST";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { expectedToken });
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = 1
            };

            _authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(expectedToken)).Return(expectedUser);

            _attribute.OnActionExecuting(_actionContext);
            ApplicationUser actualUser = ((ApiControllerBase)_actionContext.ControllerContext.Controller).CurrentUser;
            Assert.That(actualUser, Is.SameAs(expectedUser));
        }

        [Test]
        public void ItSetsTheAnonymousClientIdOnTheApplicationUser()
        {
            const string EXPECTED_TOKEN = "TEST";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { EXPECTED_TOKEN });
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = 1
            };
            _authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(EXPECTED_TOKEN)).Return(expectedUser);
            const string EXPECTED_CLIENT_ID = "some client id";
            _clientIdCalculatorMock.Expect(mock => mock.GetClientId(_request, expectedUser)).Return(EXPECTED_CLIENT_ID);

            _attribute.OnActionExecuting(_actionContext);
            ApplicationUser actualUser = ((ApiControllerBase)_actionContext.ControllerContext.Controller).CurrentUser;

            Assert.That(actualUser.AnonymousClientId, Is.EqualTo(EXPECTED_CLIENT_ID));
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenWrongToken()
        {
            const string TOKEN = "DIFFERENT";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { TOKEN });

            _authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(Arg<string>.Is.Anything)).Return(null);

            _attribute.OnActionExecuting(_actionContext);

            Assert.That(_actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = _actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(_actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenTokenExpired()
        {
            const string EXPECTED_TOKEN = "TEST";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { EXPECTED_TOKEN });

            _attribute.OnActionExecuting(_actionContext);

            Assert.That(_actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = _actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(_actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ItReturnsAnUnauthorizedHttpStatusWhenThereIsAGamingGroupIdThatDoesntMatchTheCurrentUser()
        {
            const string TOKEN = "TEST";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { TOKEN });
            const int REQUESTED_GAMING_GROUP_ID = 1;
            _actionContext.ActionArguments.Add("gamingGroupId", REQUESTED_GAMING_GROUP_ID);
            const int GAMING_GROUP_ID = -1;
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = GAMING_GROUP_ID
            };
            _authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(TOKEN)).Return(expectedUser);
            const string EXPECTED_CLIENT_ID = "some client id";
            _clientIdCalculatorMock.Expect(mock => mock.GetClientId(_request, expectedUser)).Return(EXPECTED_CLIENT_ID);

            _attribute.OnActionExecuting(_actionContext);

            Assert.That(_actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = _actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(_actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(string.Format(ApiAuthenticationAttribute.ERROR_MESSAGE_UNAUTHORIZED_TO_GAMING_GROUP, REQUESTED_GAMING_GROUP_ID)));
        }
    }
}
