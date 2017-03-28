using BusinessLogic.Models.Games;

namespace BusinessLogic.Models.Achievements
{
    public class AchievementRelatedGameDefinitionSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GamingGroupId { get; set; }
        public BoardGameGeekInfo BoardGameGeekInfo { get; set; }
    }
}
