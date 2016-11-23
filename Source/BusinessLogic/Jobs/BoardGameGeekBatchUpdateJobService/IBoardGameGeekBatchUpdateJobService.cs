namespace BusinessLogic.Jobs.BoardGameGeekBatchUpdateJobService
{
    public interface IBoardGameGeekBatchUpdateJobService
    {
        LinkOrphanGamesJobResult LinkOrphanGames();
        int RefreshAllBoardGameGeekData();
        int RefreshOutdatedBoardGameGeekData(int daysOutdated, int? maxElementsToUpdate);
    }
}