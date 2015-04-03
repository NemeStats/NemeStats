using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using NUnit.Framework;
using UI.Attributes;

namespace UI.Tests.UnitTests.AttributesTests
{
    [TestFixture]
    public class ApiModelValidationAttributeTests
    {
        [Test]
        public void ItReturnsABadRequestResponseIfTheModelStateIsNotValid()
        {
            string expectedErrorMessage = "some validation error message";
            ApiModelValidationAttribute modelValidationAttribute = new ApiModelValidationAttribute();
            HttpControllerContext controllerContext = new HttpControllerContext {Request = new HttpRequestMessage()};
            controllerContext.Request.SetConfiguration(new HttpConfiguration());
            HttpActionContext actionContext = new HttpActionContext(controllerContext, new ReflectedHttpActionDescriptor());

            actionContext.ModelState.AddModelError("error 1", expectedErrorMessage);

            modelValidationAttribute.OnActionExecuting(actionContext);

            Assert.That(actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            string actualContent = ((ObjectContent<string>)actionContext.Response.Content).Value.ToString();
            Assert.That(actualContent, Is.EqualTo("The request is invalid."));
        }
    }
}
