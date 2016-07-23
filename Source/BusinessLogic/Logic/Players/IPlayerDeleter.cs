using System.Xml;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Players
{
    public interface IPlayerDeleter
    {
        void DeletePlayer(int playerId, ApplicationUser currentUser);
    }
}