using BusinessLogic.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Linq;

namespace BusinessLogic.Logic.Users
{
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            //TODO something is really wrong here... need to revisit the DB context creation here
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<NemeStatsDbContext>()));
        }
    }
}
