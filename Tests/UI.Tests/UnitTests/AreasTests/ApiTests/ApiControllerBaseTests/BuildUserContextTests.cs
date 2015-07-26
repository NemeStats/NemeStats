//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.Http.Controllers;
//using BusinessLogic.Logic.Users;
//using BusinessLogic.Models.User;
//using Microsoft.AspNet.Identity;
//using Microsoft.Owin.Security.DataProtection;
//using NUnit.Framework;
//using Rhino.Mocks;
//using UI.Areas.Api;
//using UI.Attributes;
//using BusinessLogic.Exceptions;

//namespace UI.Tests.UnitTests.AreasTests.ApiTests.ApiControllerBaseTests
//{
//    [TestFixture]
//    public class BuildUserContextTests
//    {
//        private ApiControllerBase apiControllerBase;
//        private HttpActionContext actionContext;
//        private HttpControllerContext controllerContext;
//        private HttpRequestMessage request;
//        private IAuthTokenValidator authTokenValidatorMock;
//        private IUserStore<ApplicationUser> userStoreMock;
//        private IDataProtectionProvider dataProtectionProviderMock;
//        private ClientIdCalculator clientIdCalculatorMock;

//        [SetUp]
//        public void SetUp()
//        {
//            this.request = new HttpRequestMessage();
//            this.request.SetConfiguration(new HttpConfiguration());
//            authTokenValidatorMock = MockRepository.GenerateMock<IAuthTokenValidator>();
//            clientIdCalculatorMock = MockRepository.GenerateMock<ClientIdCalculator>();
//            controllerContext = new HttpControllerContext { Request = this.request };

//            apiControllerBase = new ApiControllerBase(authTokenValidatorMock, clientIdCalculatorMock);
//        }

//        [Test]
//        public void ItThrowsAnAuthenticationExceptionIfTheAuthTokenIsPresentButEmpty()
//        {
//            this.request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER, new[] { string.Empty });
//            var expectedException = new ApiAuthenticationException(ApiControllerBase.AUTH_HEADER);

//            var exception = Assert.Throws<ApiAuthenticationException>(() => apiControllerBase.BuildUserContext(controllerContext));

//            Assert.That(exception.Message, Is.EqualTo(expectedException.Message));
//        }
        
//        [Test]
//        public void ItThrowsAnAuthenticationExceptionIfTheAuthTokenIsNotValid()
//        {
//            this.request.Headers.Add(ApiAuthenticationAttribute.AUTH_HEADER,
//                                     new[]
//                                     {
//                                         "some invalid token"
//                                     });
//            authTokenValidatorMock.Expect(mock => mock.ValidateAuthToken(Arg<string>.Is.Anything)).Return(null);
//            var expectedException = new ApiAuthenticationException(ApiControllerBase.AUTH_HEADER);

//            var exception = Assert.Throws<ApiAuthenticationException>(() => apiControllerBase.BuildUserContext(controllerContext));

//            Assert.That(exception.Message, Is.EqualTo(expectedException.Message));
//        }
//    }
//}
