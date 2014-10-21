using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;

namespace BusinessLogic.Logic.Players
{
    public interface IPlayerSaver
    {
        Player Save(Player player, ApplicationUser currentUser);
    }
}
