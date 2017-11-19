using BusinessLogic.Models;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public interface IBoardGameGeekGameDefinitionCreator
    {
        BoardGameGeekGameDefinition CreateBoardGameGeekGameDefinition(int boardGameGeekGameDefinitionId);
    }
}
