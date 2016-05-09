using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Achievements
{
    public class AchievementAwarder// : IAchievementAwarder
    {
        //private readonly IDataContext _dataContext;

        //public AchievementAwarder(IDataContext dataContext)
        //{
        //    _dataContext = dataContext;
        //}

        //public void AwardNewAchievements(List<int> playerIds)
        //{
        //    var allAchievements = _dataContext.GetQueryable<Achievement>().ToList();
        //    foreach (var playerId in playerIds)
        //    {
        //        AwardAchievementsForPlayer(playerId, allAchievements);
        //    }
        //}

        //internal virtual void AwardAchievementsForPlayer(int playerId, List<Achievement> achievementsToCheck)
        //{
        //    foreach (var achievement in achievementsToCheck)
        //    {
        //        UpdateAchievement(playerId, achievement);
        //    }
        //}

        //internal virtual void UpdateAchievement(int playerId, Achievement achievement)
        //{
        //    var playerAchievementScore = GetPlayerAchievementScore(playerId, achievement);

        //    var existingPlayerAchievement = _dataContext.GetQueryable<PlayerAchievement>()
        //                                                .FirstOrDefault(x => x.PlayerId == playerId && x.AchievementId == achievement.Id);

        //    AchievementLevelEnum achievementLevel;
        //    if (playerAchievementScore < achievement.AchievementLevel1Threshold)
        //    {
        //        if (existingPlayerAchievement != null)
        //        {
        //            _dataContext.DeleteById<PlayerAchievement>(existingPlayerAchievement.Id, new AnonymousApplicationUser());
        //        }
        //        return;
        //    }

        //    if (playerAchievementScore < achievement.AchievementLevel2Threshold)
        //    {
        //        achievementLevel = AchievementLevelEnum.Bronze;
        //    }else if (playerAchievementScore < achievement.AchievementLevel3Threshold)
        //    {
        //        achievementLevel = AchievementLevelEnum.Silver;
        //    }
        //    else
        //    {
        //        achievementLevel = AchievementLevelEnum.Gold;
        //    }

        //    if (existingPlayerAchievement != null)
        //    {
        //        if (existingPlayerAchievement.AchievementLevel == achievementLevel)
        //        {
        //            return;
        //        }

        //        SavePlayerAchievement(playerId, achievement, achievementLevel, existingPlayerAchievement);
        //        return;
        //    }

        //    SavePlayerAchievement(playerId, achievement, achievementLevel);

        //}

        //private void SavePlayerAchievement(int playerId, Achievement achievement, AchievementLevelEnum achievementLevel, PlayerAchievement playerAchievement = null)
        //{
        //    if (playerAchievement == null)
        //    {
        //        playerAchievement = new PlayerAchievement();
        //    }
        //    playerAchievement.AchievementId = achievement.Id;
        //    playerAchievement.AchievementLevel = achievementLevel;
        //    playerAchievement.DateCreated = DateTime.UtcNow;
        //    playerAchievement.PlayerId = playerId;
        //    _dataContext.Save(playerAchievement, new AnonymousApplicationUser());
        //}

        //internal virtual int GetPlayerAchievementScore(int playerId, Achievement achievement)
        //{
        //    return _dataContext.MakeRawSqlQuery<int?>(achievement.PlayerCalculationSql, new SqlParameter("playerId", playerId)).FirstOrDefault() ?? 0;
        //}
    }
}