namespace BusinessLogic.Jobs.BoardGameGeekBatchUpdate
{
    public interface IBoardGameGeekBatchUpdateJobService
    {
        LinkOrphanGamesJobResult LinkOrphanGames();
        int RefreshAllBoardGameGeekData(int startId = 0);
        int RefreshOutdatedBoardGameGeekData(int daysOutdated, int? maxElementsToUpdate);
    }
}