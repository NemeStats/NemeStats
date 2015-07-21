using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using UI.Attributes;

namespace UI.Areas.Api
{
    public abstract class ApiControllerBase : ApiController
    {
        public virtual ApplicationUser CurrentUser { get; set; } 
    }
}