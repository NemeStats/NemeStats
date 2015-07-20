using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using Links;
using BusinessLogic.Exceptions;

namespace UI.Attributes
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception as ApiFriendlyException;
            if (exception != null)
            {
                context.Response = new HttpResponseMessage(exception.StatusCode)
                {
                    Content = new StringContent(context.Exception.Message)
                };
            }else
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An internal server error occurred. This isn't your fault. "
                     + "We have been notified of the problem and will try to fix it as soon as possible.")
                }; 
            }
        }
    }
}