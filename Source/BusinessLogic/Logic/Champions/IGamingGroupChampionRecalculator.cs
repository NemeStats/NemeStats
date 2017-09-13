namespace BusinessLogic.Logic.Champions
{
    public interface IGamingGroupChampionRecalculator
    {
        void RecalculateGamingGroupChampion(int playedGameId);
        void RecalculateGamingGroupChampionUsingGamingGroupId(int gamingGroupId);
    }
}