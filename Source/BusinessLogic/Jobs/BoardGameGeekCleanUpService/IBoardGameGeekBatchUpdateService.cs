namespace BusinessLogic.Jobs.BoardGameGeekCleanUpService
{
    public interface IBoardGameGeekBatchUpdateService
    {
        LinkOrphanGamesResult LinkOrphanGames();
        int RefreshAllBoardGameGeekData();
    }
}