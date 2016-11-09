using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Facades
{
    public interface IUniversalGameRetriever
    {
        UniversalGameData GetUniversalGameData(int boardGameGeekGameDefinitionId, ApplicationUser currentUser);
    }
}
