namespace BusinessLogic.Models.Champions
{
    public class GetRecentChampionChangesFilter
    {
        public GetRecentChampionChangesFilter(int numberOfDaysOfRecentChangesToShow, int gamingGroupId)
        {
            NumberOfDaysOfRecentChangesToShow = numberOfDaysOfRecentChangesToShow;
            GamingGroupId = gamingGroupId;
        }

        public int NumberOfDaysOfRecentChangesToShow { get; }
        public int GamingGroupId { get; }
    }
}
