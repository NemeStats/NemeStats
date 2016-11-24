using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public interface IUniversalGameRetriever
    {
        BoardGameGeekGameSummary GetBoardGameGeekGameSummary(int boardGameGeekGameDefinitionId, ApplicationUser currentUser, int numberOfRecentlyPlayedGamesToRetrieve = 5);
    }
}
