using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using Links;
using StructureMap.Diagnostics;

namespace UI.Attributes
{
    public class ApiModelValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string errorMessage = string.Empty;

            if (actionContext.ModelState.IsValid == false)
            {
                errorMessage = BuildErrorMessage(actionContext);
            }
            else if (actionContext.ActionArguments.All(x => x.Value == null))
            {
                errorMessage = "The request is invalid.";
            }

            actionContext.Response = actionContext.Request.CreateResponse(
                HttpStatusCode.BadRequest, errorMessage);
        }

        private static string BuildErrorMessage(HttpActionContext actionContext)
        {
            List<String> allModelStateErrors = new List<string>();
            foreach (ModelState modelState in actionContext.ModelState.Values)
            {
                allModelStateErrors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
            }

            string errorMessage = string.Join("|", allModelStateErrors);
            return errorMessage;
        }
    }
}