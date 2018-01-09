using BusinessLogic.Models.Players;

namespace BusinessLogic.Logic.Players
{
    public interface IHomePagePlayerSummaryRetriever
    {
        HomePagePlayerSummary GetHomePagePlayerSummaryForUser(string applicationUserId, int gamingGroupId);
    }
}
