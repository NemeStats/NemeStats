using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.GamingGroups
{
    public interface IDeleteGamingGroupComponent
    {
        void Execute(int gamingGroupId, ApplicationUser currentUser, IDataContext dataContext);
        void Execute(int gamingGroupId, ApplicationUser currentUser);
    }
}
