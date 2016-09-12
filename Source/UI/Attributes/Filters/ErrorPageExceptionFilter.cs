using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using BusinessLogic.Exceptions;
using UI.Areas.Api.Models;
using IExceptionFilter = System.Web.Mvc.IExceptionFilter;

namespace UI.Attributes.Filters
{
    public class ErrorPageExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            var response = context.HttpContext.Response;

            string errorPageHtml;

            var exception = context.Exception as ApiFriendlyException;
            if (exception != null)
            {
                response.StatusCode = (int)exception.StatusCode;

                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    errorPageHtml =
                        @"<html><head></head><body>Sorry that you got this error (and this primitive error page), 
                                                but it looks like you are trying to go to a page that either no longer exists or never existed.
                                                <a href='/'>Click here to go back to the home page</a></body></html>";
                }
                else
                {
                    var otherErrorTemplate =
                                                @"<html><head></head><body>Sorry you got this primitive error page, but something went wrong. Here is
                                                a cryptic error message for you: <b>{0}</b></br>
                                                <a href='/'>Click here to go back to the home page</a></body></html>";
                    errorPageHtml = string.Format(otherErrorTemplate, exception.Message);
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                errorPageHtml =
                                        @"<html><head></head><body>Oh crap! An error happened and it wasn't your fault. We were able to log the error
                                            so hopefully we'll be able to fix it soon. <a href='/'>Click here to go back to the home page</a></body></html>";
            }

            response.Output.Write(errorPageHtml);
            response.End();
        }
    }
}