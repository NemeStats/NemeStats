using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public interface ICacheableGameDataRetriever
    {
        CacheableGameData GetResults(int boardGameGeekGameDefinitionId);
    }
}