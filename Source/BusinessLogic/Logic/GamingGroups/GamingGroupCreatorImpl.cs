using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupCreatorImpl : GamingGroupCreator
    {
        internal const string EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK = "GamingGroup name cannot be null or blank";

        private DataContext dataContext;
        private UserManager<ApplicationUser> userManager;

        public GamingGroupCreatorImpl(DataContext dataContext, UserManager<ApplicationUser> userManager)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
        }

        public async Task<GamingGroup> CreateGamingGroupAsync(string gamingGroupName, ApplicationUser currentUser)
        {
            ValidateGamingGroupName(gamingGroupName);

            GamingGroup gamingGroup = new GamingGroup()
            {
                OwningUserId = currentUser.Id,
                Name = gamingGroupName
            };

            GamingGroup returnGroup = dataContext.Save<GamingGroup>(gamingGroup, currentUser);
            ApplicationUser user = await userManager.FindByIdAsync(currentUser.Id);
            user.CurrentGamingGroupId = returnGroup.Id;
            await userManager.UpdateAsync(user);
            return returnGroup;
        }

        private static void ValidateGamingGroupName(string gamingGroupName)
        {
            if (string.IsNullOrWhiteSpace(gamingGroupName))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK);
            }
        }
    }
}
