using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public class PlayerAchievementRetriever : IPlayerAchievementRetriever
    {
        private readonly IDataContext _dataContext;
        private readonly IAchievementRetriever _achievementRetriever;
        private readonly IPlayerRetriever _playerRetriever;
        private readonly IBoardGameGeekGameDefinitionInfoRetriever _boardGameGeekGameDefinitionInfoRetriever;

        public PlayerAchievementRetriever(IDataContext dataContext, 
            IAchievementRetriever achievementRetriever, 
            IBoardGameGeekGameDefinitionInfoRetriever boardGameGeekGameDefinitionInfoRetriever, 
            IPlayerRetriever playerRetriever)
        {
            _dataContext = dataContext;
            _achievementRetriever = achievementRetriever;
            _boardGameGeekGameDefinitionInfoRetriever = boardGameGeekGameDefinitionInfoRetriever;
            _playerRetriever = playerRetriever;
        }

        public virtual PlayerAchievement GetPlayerAchievement(int playerId, AchievementId achievementId)
        {
            return _dataContext
                .GetQueryable<PlayerAchievement>()
                .Include(pa => pa.Player)
                .FirstOrDefault(pa => pa.AchievementId == achievementId && pa.PlayerId == playerId);
        }

        public virtual PlayerAchievementDetails GetCurrentPlayerAchievementDetails(AchievementId achievementId, ApplicationUser currentUser)
        {
            var achievement = _achievementRetriever.GetAchievement(achievementId);

            var result = new PlayerAchievementDetails
            {
                AchievementId = achievementId,
                AchievementDescription = achievement.Description,
                AchievementIconClass = achievement.IconClass,
                AchievementName = achievement.Name,
                LevelThresholds = achievement.LevelThresholds
            };

            result.NumberOfPlayersWithThisAchievement = _dataContext.GetQueryable<PlayerAchievement>().Count(y => y.AchievementId == achievementId);

            if (currentUser.UserName == AnonymousApplicationUser.USER_NAME_ANONYMOUS)
            {
                return result;
            }

            var playerForCurrentUser = _playerRetriever.GetPlayerForCurrentUser(currentUser.Id, currentUser.CurrentGamingGroupId);

            if (playerForCurrentUser != null)
            {
                result.PlayerId = playerForCurrentUser.Id;
                result.PlayerName = playerForCurrentUser.Name;

                var achievementAwarded = achievement.IsAwardedForThisPlayer(playerForCurrentUser.Id);

                result.AchievementLevel = achievementAwarded.LevelAwarded;
                result.PlayerProgress = achievementAwarded.PlayerProgress;

                SetRelatedEntities(achievement.Group, result, achievementAwarded.RelatedEntities);
            }

            var playerAchievement = _dataContext
                .GetQueryable<PlayerAchievement>()
                .FirstOrDefault(x => x.AchievementId == achievementId && x.Player.ApplicationUserId == currentUser.Id);

            if (playerAchievement == null)
            {
                return result;
            }

            result.DateCreated = playerAchievement.DateCreated;
            result.LastUpdatedDate = playerAchievement.LastUpdatedDate;
            
            return result;
        }

        internal virtual void SetRelatedEntities(AchievementGroup achievementGroup, PlayerAchievementDetails result, List<int> relatedEntityIds)
        {
            switch (achievementGroup)
            {
                case AchievementGroup.Game:
                    result.RelatedGameDefinitions = _dataContext.GetQueryable<GameDefinition>()
                        .Where(x => relatedEntityIds.Contains(x.Id))
                        .Select(x => new AchievementRelatedGameDefinitionSummary
                        {
                            Id = x.Id,
                            Name = x.Name,
                            GamingGroupId = x.GamingGroupId,
                            BoardGameGeekInfo = x.BoardGameGeekGameDefinitionId == null
                                ? null
                                : _boardGameGeekGameDefinitionInfoRetriever.GetResults(x.BoardGameGeekGameDefinitionId.Value),
                        })
                        .OrderBy(x => x.Name)
                        .ToList();
                    break;
                case AchievementGroup.PlayedGame:
                    result.RelatedPlayedGames = _dataContext.GetQueryable<PlayedGame>()
                        .Where(x => relatedEntityIds.Contains(x.Id))
                        .Select(x => new AchievementRelatedPlayedGameSummary
                        {
                            GameDefinitionId = x.GameDefinitionId,
                            WinnerType = x.WinnerType,
                            DatePlayed = x.DatePlayed,
                            BoardGameGeekGameDefinitionId = x.GameDefinition.BoardGameGeekGameDefinitionId,
                            GameDefinitionName = x.GameDefinition.Name,
                            PlayedGameId = x.Id,
                            ThumbnailImageUrl =
                                x.GameDefinition.BoardGameGeekGameDefinition == null ? null : x.GameDefinition.BoardGameGeekGameDefinition.Thumbnail
                        })
                        .OrderByDescending(x => x.DatePlayed)
                        .ToList();
                    break;
                case AchievementGroup.Player:
                    result.RelatedPlayers = _dataContext.GetQueryable<Player>()
                        .Where(x => relatedEntityIds.Contains(x.Id))
                        .Select(x => new AchievementRelatedPlayerSummary
                        {
                            PlayerId = x.Id,
                            PlayerName = x.Name,
                            GamingGroupId = x.GamingGroupId,
                            GamingGroupName = x.GamingGroup.Name
                        }).ToList();
                    break;
                case AchievementGroup.NotApplicable:
                    break;
            }
        }
    }
}
