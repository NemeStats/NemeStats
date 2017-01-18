using BusinessLogic.DataAccess;

namespace BusinessLogic.Logic.PlayedGames
{
    public interface IApplicationLinker
    {
        void LinkApplication(int playedGameId, string applicationName, string entityId, IDataContext dataContext);
    }
}
