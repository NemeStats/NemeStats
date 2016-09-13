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
            var response = context.HttpContext.Response;

            var exception = context.Exception as ApiFriendlyException;
            if (exception != null)
            {
                response.StatusCode = (int)exception.StatusCode;
                throw new HttpException((int)exception.StatusCode, exception.Message);
            }

            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            throw new HttpException((int)HttpStatusCode.InternalServerError, exception.Message);
        }
    }
}