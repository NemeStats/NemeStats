using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AutoMapper;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Infrastructure;
using UI.Areas.Api.Models;
using UI.Attributes;
using UI.Models.API;

namespace UI.Areas.Api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserRegisterer userRegisterer;
        private readonly IAuthTokenGenerator authTokenGenerator;

        public UsersController(IUserRegisterer userRegisterer, IAuthTokenGenerator authTokenGenerator)
        {
            this.userRegisterer = userRegisterer;
            this.authTokenGenerator = authTokenGenerator;
        }

        [ApiRoute("Users")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> RegisterNewUser(NewUserMessage newUserMessage)
        {
            NewUser newUser = Mapper.Map<NewUserMessage, NewUser>(newUserMessage);

		    RegisterNewUserResult registerNewUserResult = await this.userRegisterer.RegisterUser(newUser);

            if (registerNewUserResult.Result.Succeeded)
            {
                string authToken = authTokenGenerator.GenerateAuthToken(registerNewUserResult.NewlyRegisteredUser.UserId);
                NewlyRegisteredUserMessage newlyRegisteredUserMessage = Mapper.Map<NewlyRegisteredUser, NewlyRegisteredUserMessage>(registerNewUserResult.NewlyRegisteredUser);
                newlyRegisteredUserMessage.AuthenticationToken = authToken;
                return Request.CreateResponse(HttpStatusCode.OK, newlyRegisteredUserMessage);
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, registerNewUserResult.Result.Errors.First());
        }
    }
}