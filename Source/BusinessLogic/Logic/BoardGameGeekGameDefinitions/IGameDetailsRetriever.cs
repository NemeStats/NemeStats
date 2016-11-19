using BoardGameGeekApiClient.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public interface IGameDetailsRetriever
    {
        GameDetails GetGameData(int boardGameGeekGameDefinitionId, ApplicationUser currentUser);
    }
}
