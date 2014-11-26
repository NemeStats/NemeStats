using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public class GamingGroupContextSwitcher : IGamingGroupContextSwitcher
    {
        public const string EXCEPTION_MESSAGE_NO_ACCESS = "User with Id '{0}' does not have access to GamingGroup with Id '{1}'";

        private readonly IDataContext dataContext;

        public GamingGroupContextSwitcher(IDataContext dataContextMock)
        {
            this.dataContext = dataContextMock;
        }

        public void SwitchGamingGroupContext(int gamingGroupId, ApplicationUser currentUser)
        {
            if (gamingGroupId == currentUser.CurrentGamingGroupId.Value)
            {
                return;
            }

            bool hasAccess  = dataContext.GetQueryable<UserGamingGroup>()
                                         .Any(userGamingGroup => userGamingGroup.ApplicationUserId == currentUser.Id
                                                                 && userGamingGroup.GamingGroupId == gamingGroupId);

            if (!hasAccess)
            {
                throw new UnauthorizedAccessException(string.Format(EXCEPTION_MESSAGE_NO_ACCESS, currentUser.Id, gamingGroupId));
            }

            var user = dataContext.FindById<ApplicationUser>(currentUser.Id);
            user.CurrentGamingGroupId = gamingGroupId;
            dataContext.Save(user, currentUser);
        }
    }
}
