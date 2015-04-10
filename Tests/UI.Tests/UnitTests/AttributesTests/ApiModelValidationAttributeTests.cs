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
            ApiModelValidationAttribute modelValidationAttribute = new ApiModelValidationAttribute();
            HttpControllerContext controllerContext = new HttpControllerContext {Request = new HttpRequestMessage()};
            controllerContext.Request.SetConfiguration(new HttpConfiguration());
            HttpActionContext actionContext = new HttpActionContext(controllerContext, new ReflectedHttpActionDescriptor());

            const string EXPECTED_ERROR_MESSAGE1 = "some validation error message 1";
            actionContext.ModelState.AddModelError("error 1", EXPECTED_ERROR_MESSAGE1);
            const string EXPECTED_ERROR_MESSAGE2 = "some validation error message 2";
            actionContext.ModelState.AddModelError("error 2", EXPECTED_ERROR_MESSAGE2);

            modelValidationAttribute.OnActionExecuting(actionContext);

            Assert.That(actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            string actualContent = ((ObjectContent<string>)actionContext.Response.Content).Value.ToString();
            Assert.That(actualContent, Is.EqualTo(EXPECTED_ERROR_MESSAGE1 + "|" + EXPECTED_ERROR_MESSAGE2));
        }
    }
}
