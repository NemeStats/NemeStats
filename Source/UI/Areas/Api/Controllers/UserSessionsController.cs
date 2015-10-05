using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BusinessLogic.Logic.Users;
using UI.Areas.Api.Models;
using UI.Attributes;

namespace UI.Areas.Api.Controllers
{
    public class UserSessionsController : ApiControllerBase
    {
        private readonly ApplicationUserManager applicationUserManager;
        private readonly IAuthTokenGenerator authTokenGenerator;

        public UserSessionsController(ApplicationUserManager applicationUserManager, IAuthTokenGenerator authTokenGenerator)
        {
            this.applicationUserManager = applicationUserManager;
            this.authTokenGenerator = authTokenGenerator;
        }

        [HttpPost]
        [ApiModelValidationAttribute]
        [ApiRoute("UserSessions/")]
        public async Task<HttpResponseMessage> Login(CredentialsMessage credentialsMessage)
        {
            var user = await applicationUserManager.FindAsync(credentialsMessage.UserName, credentialsMessage.Password);
            if (user == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid credentials provided.");
            }

            NewAuthTokenMessage newAuthTokenMessage = new NewAuthTokenMessage
            {
                AuthenticationToken = authTokenGenerator.GenerateAuthToken(user.Id)
            };
            
            return Request.CreateResponse(HttpStatusCode.OK, newAuthTokenMessage);
        }
    }
}
