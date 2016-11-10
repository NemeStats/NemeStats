using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.UniversalGameDefinitions
{
    public interface IUniversalGameRetriever
    {
        UniversalGameData GetResults(int boardGameGeekGameDefinitionId);
    }
}
