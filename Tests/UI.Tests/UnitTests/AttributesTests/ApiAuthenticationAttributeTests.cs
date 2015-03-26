using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Attributes;

namespace UI.Tests.UnitTests.AttributesTests
{
    [TestFixture]
    public class ApiAuthenticationAttributeTests
    {
        private ApiAuthenticationAttribute _attribute;
        private HttpActionContext _actionContext;
        private HttpControllerContext _controllerContext;
        private HttpRequestMessage _request;
        private ApplicationUserManager _applicationUserManager;
        private IUserStore<ApplicationUser> _userStoreMock;
        private IDataProtectionProvider _dataProtectionProviderMock;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            _request = new HttpRequestMessage();
            _userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            _dataProtectionProviderMock = MockRepository.GenerateMock<IDataProtectionProvider>();
            _dataProtectionProviderMock.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);
            _applicationUserManager = MockRepository.GenerateMock<ApplicationUserManager>(_userStoreMock, _dataProtectionProviderMock);
            _attribute = new ApiAuthenticationAttribute(_applicationUserManager);
            _controllerContext = new HttpControllerContext {Request = _request};
            _actionContext = new HttpActionContext(_controllerContext, new ReflectedHttpActionDescriptor());
        }

        [Test]
        public void ShouldReturnBadRequestWhenNoTokenProvided()
        {
            _attribute.OnActionExecuting(_actionContext);
            Assert.AreEqual(HttpStatusCode.BadRequest, _actionContext.Response.StatusCode);
        }

        [Test]
        public void ShouldReturnBadRequestWhenTokenHasEmptyValue()
        {
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new [] { string.Empty });
            _attribute.OnActionExecuting(_actionContext);
            Assert.AreEqual(HttpStatusCode.BadRequest, _actionContext.Response.StatusCode);
        }

        [Test]
        public void ShouldGetTheUsersToken()
        {
            const string expectedToken = "TEST";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { expectedToken });
            var expectedUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    AuthenticationToken = expectedToken
                }
            };

            _applicationUserManager.Expect(x => x.Users)
                .Repeat.Any()
                .Return(expectedUsers.AsQueryable());

            _attribute.OnActionExecuting(_actionContext);
            _applicationUserManager.AssertWasCalled(x => x.Users);
        }

        [Test]
        public void ShouldReturnUnauthorizedHttpStatusWhenWronToken()
        {
            const string expectedToken = "TEST";
            const string actualToken = "DIFFERENT";
            _request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { expectedToken });
            var expectedUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    AuthenticationToken = actualToken
                }
            };

            _applicationUserManager.Expect(x => x.Users)
                .Repeat.Any()
                .Return(expectedUsers.AsQueryable());

            _attribute.OnActionExecuting(_actionContext);
            Assert.AreEqual(HttpStatusCode.Unauthorized, _actionContext.Response.StatusCode);
        }
    }
}
