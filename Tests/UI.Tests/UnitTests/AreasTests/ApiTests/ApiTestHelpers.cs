using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;

namespace UI.Tests.UnitTests.AreasTests.ApiTests
{
    public class AssertThatApiAction
    {
        public static void HasThisError(HttpResponseMessage responseMessage, HttpStatusCode expectedStatusCode, string expectedMessage)
        {
            Assert.That(responseMessage.Content, Is.TypeOf(typeof(ObjectContent<HttpError>)));
            var content = responseMessage.Content as ObjectContent<HttpError>;
            var httpError = content.Value as HttpError;
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedStatusCode));
            Assert.That(httpError.Message, Is.EqualTo(expectedMessage));
        }

        public static T ReturnsThisTypeWithThisStatusCode<T>(HttpResponseMessage responseMessage, HttpStatusCode expectedStatusCode) where T: class
        {
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedStatusCode));
            Assert.That(responseMessage.Content, Is.TypeOf(typeof(ObjectContent<T>)));
            var content = responseMessage.Content as ObjectContent<T>;
            return content.Value as T;
        }

        public static void ReturnsANoContentResponseWithNoContent(HttpResponseMessage responseMessage)
        {
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(responseMessage.Content, Is.Null);
        }
    }
}
