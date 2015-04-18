using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
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
        private IAuthTokenValidator authTokenValidatorMock;
        private IUserStore<ApplicationUser> _userStoreMock;
        private IDataProtectionProvider _dataProtectionProviderMock;
        private ClientIdCalculator clientIdCalculatorMock;

        [SetUp]
        public void SetUp()
        {
            _request = new HttpRequestMessage();
            _request.SetConfiguration(new HttpConfiguration());
            authTokenValidatorMock = MockRepository.GenerateMock<IAuthTokenValidator>();
            clientIdCalculatorMock = MockRepository.GenerateMock<ClientIdCalculator>();
            _attribute = new ApiAuthenticationAttribute(authTokenValidatorMock, clientIdCalculatorMock);
            _controllerContext = new HttpControllerContext {Request = _request};
            _actionContext = new HttpActionContext(_controllerContext, new ReflectedHttpActionDescriptor());
        }

        [Test]
        public void ShouldReturnBadRequestWhenNoTokenProvided()
        {
            _attribute.OnActionExecuting(_actionContext);
            Assert.AreEqual(HttpStatusCode.BadRequest, _actionContext.Response.StatusCode);
            string actualContent = ((ObjectContent<string>)_actionContext.Response.Content).Value.ToString();
            Assert.AreEqual("This action requires an X-Auth-Token header.", actualContent);
        }

        [Test]
        public void ShouldReturnBadRequestWhenTokenHasEmptyValue()
        {
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new [] { string.Empty });
            _attribute.OnActionExecuting(_actionContext);
            Assert.AreEqual(HttpStatusCode.BadRequest, _actionContext.Response.StatusCode);
            string actualContent = ((ObjectContent<string>)_actionContext.Response.Content).Value.ToString();
            Assert.AreEqual("Invalid X-Auth-Token", actualContent);
        }

        [Test]
        public void ItSetsTheApplicationUserOnTheActionContextIfTheAuthTokenIsValid()
        {
            const string expectedToken = "TEST";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { expectedToken });
            var expectedUser = new ApplicationUser
            {
                Id = "some id"
            };

            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(expectedToken)).Return(expectedUser);

            _attribute.OnActionExecuting(_actionContext);
            ApplicationUser actualUser = _actionContext.ActionArguments[ApiAuthenticationAttribute.ACTION_ARGUMENT_APPLICATION_USER] as ApplicationUser;
            Assert.That(actualUser, Is.SameAs(expectedUser));
        }

        [Test]
        public void ItSetsTheAnonymousClientIdOnTheApplicationUser()
        {
            const string EXPECTED_TOKEN = "TEST";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { EXPECTED_TOKEN });
            var expectedUser = new ApplicationUser
            {
                Id = "some id"
            };
            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(EXPECTED_TOKEN)).Return(expectedUser);
            const string EXPECTED_CLIENT_ID = "some client id";
            clientIdCalculatorMock.Expect(mock => mock.GetClientId(_request, expectedUser)).Return(EXPECTED_CLIENT_ID);

            _attribute.OnActionExecuting(_actionContext);
            ApplicationUser actualUser = _actionContext.ActionArguments[ApiAuthenticationAttribute.ACTION_ARGUMENT_APPLICATION_USER] as ApplicationUser;

            Assert.That(actualUser.AnonymousClientId, Is.EqualTo(EXPECTED_CLIENT_ID));
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenWrongToken()
        {
            const string TOKEN = "DIFFERENT";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { TOKEN });

            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(Arg<string>.Is.Anything)).Return(null);

            _attribute.OnActionExecuting(_actionContext);

            Assert.AreEqual(HttpStatusCode.Unauthorized, _actionContext.Response.StatusCode);
            string actualContent = ((ObjectContent<string>)_actionContext.Response.Content).Value.ToString();
            Assert.AreEqual("Invalid X-Auth-Token", actualContent);
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenTokenExpired()
        {
            const string EXPECTED_TOKEN = "TEST";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { EXPECTED_TOKEN });

            _attribute.OnActionExecuting(_actionContext);

            Assert.AreEqual(HttpStatusCode.Unauthorized, _actionContext.Response.StatusCode);
            string actualContent = ((ObjectContent<string>)_actionContext.Response.Content).Value.ToString();
            Assert.AreEqual("Invalid X-Auth-Token", actualContent);
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
            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(TOKEN)).Return(expectedUser);
            const string EXPECTED_CLIENT_ID = "some client id";
            clientIdCalculatorMock.Expect(mock => mock.GetClientId(_request, expectedUser)).Return(EXPECTED_CLIENT_ID);

            _attribute.OnActionExecuting(_actionContext);

            Assert.AreEqual(HttpStatusCode.Unauthorized, _actionContext.Response.StatusCode);
            string actualContent = ((ObjectContent<string>)_actionContext.Response.Content).Value.ToString();
            Assert.AreEqual("User does not have access to Gaming Group with Id '" + REQUESTED_GAMING_GROUP_ID + "'.", actualContent);
        }
    }
}
