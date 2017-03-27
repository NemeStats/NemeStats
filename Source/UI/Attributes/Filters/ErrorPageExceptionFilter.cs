using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Exceptions;
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
                throw new HttpException((int)exception.StatusCode, exception.Message, context.Exception);
            }

            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            throw new HttpException((int)HttpStatusCode.InternalServerError, context.Exception.Message, context.Exception);
        }
    }
}