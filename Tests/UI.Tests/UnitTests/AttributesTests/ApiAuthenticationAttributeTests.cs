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
        private ApiAuthenticationAttribute attribute;
        private HttpActionContext actionContext;
        private HttpControllerContext controllerContext;
        private HttpRequestMessage request;
        private IAuthTokenValidator authTokenValidatorMock;
        private ClientIdCalculator clientIdCalculatorMock;

        [SetUp]
        public void SetUp()
        {
            this.request = new HttpRequestMessage();
            this.request.SetConfiguration(new HttpConfiguration());
            authTokenValidatorMock = MockRepository.GenerateMock<IAuthTokenValidator>();
            clientIdCalculatorMock = MockRepository.GenerateMock<ClientIdCalculator>();
            this.attribute = new ApiAuthenticationAttribute(authTokenValidatorMock, clientIdCalculatorMock);
            var controllerMock = MockRepository.GeneratePartialMock<ApiControllerBase>();
            this.controllerContext = new HttpControllerContext {Request = this.request, Controller = controllerMock };
            this.actionContext = new HttpActionContext(this.controllerContext, new ReflectedHttpActionDescriptor());
        }

        [Test]
        public void ItThrowsAnInvalidOperationExceptionIfTheAttributeIsAppliedToAControllerThatIsntApiControllerBase()
        {
            controllerContext.Controller = MockRepository.GenerateMock<ApiController>();

            var actualException = Assert.Throws<InvalidOperationException>(() => this.attribute.OnActionExecuting(this.actionContext));
            Assert.That(actualException.Message, 
                Is.EqualTo("The ApiAuthentication attribute can only be applied to actions in an ApiController that extends ApiControllerBase."));
        }


        [Test]
        public void ShouldReturnBadRequestWhenNoTokenProvided()
        {
            this.attribute.OnActionExecuting(this.actionContext);

            Assert.That(this.actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = this.actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(this.actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_MISSING_AUTH_TOKEN_HEADER));
        }

        [Test]
        public void ShouldReturnBadRequestWhenTokenHasEmptyValue()
        {
            this.request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new [] { string.Empty });
            this.attribute.OnActionExecuting(this.actionContext);

            Assert.That(this.actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = this.actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(this.actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ItSetsTheApplicationUserOnTheApiControllerBase()
        {
            const string expectedToken = "TEST";
            this.request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { expectedToken });
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = 1
            };

            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(expectedToken)).Return(expectedUser);

            this.attribute.OnActionExecuting(this.actionContext);
            ApplicationUser actualUser = ((ApiControllerBase)this.actionContext.ControllerContext.Controller).CurrentUser;
            Assert.That(actualUser, Is.SameAs(expectedUser));
        }

        [Test]
        public void ItSetsTheAnonymousClientIdOnTheApplicationUser()
        {
            const string EXPECTED_TOKEN = "TEST";
            this.request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { EXPECTED_TOKEN });
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = 1
            };
            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(EXPECTED_TOKEN)).Return(expectedUser);
            const string EXPECTED_CLIENT_ID = "some client id";
            clientIdCalculatorMock.Expect(mock => mock.GetClientId(this.request, expectedUser)).Return(EXPECTED_CLIENT_ID);

            this.attribute.OnActionExecuting(this.actionContext);
            ApplicationUser actualUser = ((ApiControllerBase)this.actionContext.ControllerContext.Controller).CurrentUser;

            Assert.That(actualUser.AnonymousClientId, Is.EqualTo(EXPECTED_CLIENT_ID));
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenWrongToken()
        {
            const string TOKEN = "DIFFERENT";
            this.request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { TOKEN });

            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(Arg<string>.Is.Anything)).Return(null);

            this.attribute.OnActionExecuting(this.actionContext);

            Assert.That(this.actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = this.actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(this.actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenTokenExpired()
        {
            const string EXPECTED_TOKEN = "TEST";
            this.request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { EXPECTED_TOKEN });

            this.attribute.OnActionExecuting(this.actionContext);

            Assert.That(this.actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = this.actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(this.actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(ApiAuthenticationAttribute.ERROR_MESSAGE_INVALID_AUTH_TOKEN));
        }

        [Test]
        public void ItReturnsAnUnauthorizedHttpStatusWhenThereIsAGamingGroupIdThatDoesntMatchTheCurrentUser()
        {
            const string TOKEN = "TEST";
            this.request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { TOKEN });
            const int REQUESTED_GAMING_GROUP_ID = 1;
            this.actionContext.ActionArguments.Add("gamingGroupId", REQUESTED_GAMING_GROUP_ID);
            const int GAMING_GROUP_ID = -1;
            var expectedUser = new ApplicationUser
            {
                Id = "some id",
                CurrentGamingGroupId = GAMING_GROUP_ID
            };
            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(TOKEN)).Return(expectedUser);
            const string EXPECTED_CLIENT_ID = "some client id";
            clientIdCalculatorMock.Expect(mock => mock.GetClientId(this.request, expectedUser)).Return(EXPECTED_CLIENT_ID);

            this.attribute.OnActionExecuting(this.actionContext);

            Assert.That(this.actionContext.Response.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = this.actionContext.Response.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(this.actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(httpError.Message, Is.EqualTo(string.Format(ApiAuthenticationAttribute.ERROR_MESSAGE_UNAUTHORIZED_TO_GAMING_GROUP, REQUESTED_GAMING_GROUP_ID)));
        }
    }
}
