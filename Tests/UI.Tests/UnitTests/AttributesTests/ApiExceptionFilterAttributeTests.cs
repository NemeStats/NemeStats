using System.Net.Http;
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
        private RhinoAutoMocker<ApiExceptionFilterAttribute> _autoMocker;
        private HttpActionExecutedContext _context;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<ApiExceptionFilterAttribute>();

            _context = new HttpActionExecutedContext
            {
                ActionContext = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Request = new HttpRequestMessage()
                    }

                }
            };
            _context.Request.SetConfiguration(new HttpConfiguration());
        }

        [Test]
        public async Task ItReturnsTheStatusCodeAndMessageIfItIsHandlingAnApiExceptionFilter()
        {
            var expectedException = new EntityDoesNotExistException<Player>("some id");
            _context.Exception = expectedException;

            _autoMocker.ClassUnderTest.OnException(_context);

            Assert.That(_context.Response.StatusCode, Is.EqualTo(expectedException.StatusCode));
            Assert.That(await _context.Response.Content.ReadAsStringAsync(),Does.Contain(expectedException.Message));
        }
    }
}
