using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;
using UI.Attributes;
using UI.Transformations;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    public abstract class ApiControllerTestBase<T> where T : ApiController
    {
        protected RhinoAutoMocker<T> autoMocker;
        protected ApplicationUser applicationUser;
        protected const int EXPECTED_GAMING_GROUP_ID = 2;

        [SetUp]
        public void BaseSetUp()
        {
            AutomapperConfiguration.Configure();
            this.autoMocker = new RhinoAutoMocker<T>();
            var controllerContextMock = MockRepository.GeneratePartialMock<HttpControllerContext>();
            var actionDescriptorMock = MockRepository.GenerateMock<HttpActionDescriptor>();
            this.autoMocker.ClassUnderTest.ActionContext = new HttpActionContext(controllerContextMock, actionDescriptorMock);

            this.applicationUser = new ApplicationUser
            {
                Id = "application user id",
                CurrentGamingGroupId = EXPECTED_GAMING_GROUP_ID
            };
            autoMocker.ClassUnderTest.ActionContext.ActionArguments[ApiAuthenticationAttribute.ACTION_ARGUMENT_APPLICATION_USER] = applicationUser;
            this.autoMocker.ClassUnderTest.Request = new HttpRequestMessage();
            this.autoMocker.ClassUnderTest.Request.SetConfiguration(new HttpConfiguration());
        }
    }
}
