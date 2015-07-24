using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using NUnit.Framework;
using StructureMap.AutoMocking;
using UI.Attributes;

namespace UI.Tests.UnitTests.AttributesTests
{
    [TestFixture]
    public class ApiExceptionFilterAttributeTests
    {
        private RhinoAutoMocker<ApiExceptionFilterAttribute> autoMocker;
        private HttpActionExecutedContext context;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<ApiExceptionFilterAttribute>();

            context = new HttpActionExecutedContext
            {
                ActionContext = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Request = new HttpRequestMessage()
                    }

                }
            };
            context.Request.SetConfiguration(new HttpConfiguration());
        }

        [Test]
        public async Task ItReturnsTheStatusCodeAndMessageIfItIsHandlingAnApiExceptionFilter()
        {
            var expectedException = new EntityDoesNotExistException(typeof(Player), "some id");
            context.Exception = expectedException;

            autoMocker.ClassUnderTest.OnException(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo(expectedException.StatusCode));
            Assert.That(await context.Response.Content.ReadAsStringAsync(), Is.StringContaining(expectedException.Message));
        }
    }
}
