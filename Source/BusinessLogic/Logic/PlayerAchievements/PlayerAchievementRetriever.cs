using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public class PlayerAchievementRetriever : IPlayerAchievementRetriever
    {
        private readonly IDataContext _dataContext;

        public PlayerAchievementRetriever(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public virtual PlayerAchievement GetPlayerAchievement(int playerId, AchievementId achievementId)
        {
            return _dataContext
                .GetQueryable<PlayerAchievement>()
                .Include(pa => pa.Player)
                .FirstOrDefault(pa => pa.AchievementId == achievementId && pa.PlayerId == playerId);
        }
    }
}
