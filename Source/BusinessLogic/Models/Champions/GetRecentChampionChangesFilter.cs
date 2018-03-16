namespace BusinessLogic.Models.Champions
{
    public class GetRecentChampionChangesFilter
    {
        public GetRecentChampionChangesFilter(int gamingGroupId, int numberOfRecentChangesToShow)
        {
            NumberOfRecentChangesToShow = numberOfRecentChangesToShow;
            GamingGroupId = gamingGroupId;
        }

        public int NumberOfRecentChangesToShow { get; }
        public int GamingGroupId { get; }
    }
}
