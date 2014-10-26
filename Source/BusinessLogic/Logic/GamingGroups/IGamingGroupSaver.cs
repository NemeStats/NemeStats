using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GamingGroups
{
    public interface IGamingGroupSaver
    {
        Task<GamingGroup> CreateNewGamingGroup(string gamingGroupName, ApplicationUser currentUser);
        GamingGroup UpdateGamingGroupName(string gamingGroupName, ApplicationUser currentUser);
    }
}
