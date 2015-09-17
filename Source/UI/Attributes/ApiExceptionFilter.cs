using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using BusinessLogic.Exceptions;
using UI.Areas.Api.Models;

namespace UI.Attributes
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception as ApiFriendlyException;
            if (exception != null)
            {
                context.Response = context.Request.CreateResponse(exception.StatusCode, new GenericErrorMessage(exception.Message));
            }else
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, 
                    new GenericErrorMessage("An internal server error occurred. This isn't your fault. "
                     + "We have been notified of the problem and will try to fix it as soon as possible."));
            }
        }
    }
}