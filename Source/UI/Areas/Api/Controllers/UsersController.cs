using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using UI.Areas.Api.Models;
using UI.Attributes;
using UI.Transformations;
using VersionedRestApi;

namespace UI.Areas.Api.Controllers
{
    public class UsersController : ApiControllerBase
    {
        private readonly IUserRegisterer userRegisterer;
        private readonly IAuthTokenGenerator authTokenGenerator;
        private readonly IUserRetriever userRetriever;
        private readonly ITransformer transformer;

        public UsersController(IUserRegisterer userRegisterer, IAuthTokenGenerator authTokenGenerator, IUserRetriever userRetriever, ITransformer transformer)
        {
            this.userRegisterer = userRegisterer;
            this.authTokenGenerator = authTokenGenerator;
            this.userRetriever = userRetriever;
            this.transformer = transformer;
        }

        [ApiRoute("Users/")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> RegisterNewUser(NewUserMessage newUserMessage)
        {
            var newUser = Mapper.Map<NewUserMessage, NewUser>(newUserMessage);

		    var registerNewUserResult = await this.userRegisterer.RegisterUser(newUser);

            if (registerNewUserResult.Result.Succeeded)
            {
                var authToken = authTokenGenerator.GenerateAuthToken(registerNewUserResult.NewlyRegisteredUser.UserId, newUserMessage.UniqueDeviceId);
                var newlyRegisteredUserMessage = Mapper.Map<NewlyRegisteredUser, NewlyRegisteredUserMessage>(registerNewUserResult.NewlyRegisteredUser);
                newlyRegisteredUserMessage.AuthenticationToken = authToken.AuthenticationTokenString;
                newlyRegisteredUserMessage.AuthenticationTokenExpirationDateTime =
                    authToken.AuthenticationTokenExpirationDateTime;
                return Request.CreateResponse(HttpStatusCode.OK, newlyRegisteredUserMessage);
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, registerNewUserResult.Result.Errors.First());
        }

        [ApiRoute("Users/{userId}/")]
        [HttpGet]
        [ApiAuthentication]
        public virtual HttpResponseMessage GetUserInformation(string userId)
        {
            var userInformation = userRetriever.RetrieveUserInformation(CurrentUser);
            var userInformationMessage = this.transformer.Transform<UserInformationMessage>(userInformation);
            return Request.CreateResponse(HttpStatusCode.OK, userInformationMessage);
        }
    }
}