namespace BusinessLogic.Jobs.BoardGameGeekBatchUpdate
{
    public interface IBoardGameGeekBatchUpdateJobService
    {
        LinkOrphanGamesJobResult LinkOrphanGames();
        int RefreshAllBoardGameGeekData();
        int RefreshOutdatedBoardGameGeekData(int daysOutdated, int? maxElementsToUpdate);
    }
}