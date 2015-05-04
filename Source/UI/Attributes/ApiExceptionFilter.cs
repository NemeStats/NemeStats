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
            if (context.Exception is EntityDoesNotExistException 
                || context.Exception is UnauthorizedEntityAccessException
                || context.Exception is PlayerAlreadyExistsException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
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