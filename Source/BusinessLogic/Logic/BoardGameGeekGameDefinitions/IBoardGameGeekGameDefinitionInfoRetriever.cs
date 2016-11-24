using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public interface IBoardGameGeekGameDefinitionInfoRetriever
    {
        BoardGameGeekInfo GetResults(int boardGameGeekGameDefinitionId);
    }
}