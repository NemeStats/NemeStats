using System.Linq;
using System.Web.Http;
using BusinessLogic.Models.User;

namespace UI.Areas.Api
{
    public abstract class ApiControllerBase : ApiController
    {
        public virtual ApplicationUser CurrentUser { get; set; } 
    }
}