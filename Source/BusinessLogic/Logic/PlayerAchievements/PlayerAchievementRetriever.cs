using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
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

        [Obsolete("will replace all calls with a call to GetPlayerAchievement()")]
        public virtual PlayerAchievementDetails GetCurrentPlayerAchievementDetails(AchievementId achievementId, ApplicationUser currentUser)
        {
            var achievement = _achievementRetriever.GetAchievement(achievementId);

            var result = new PlayerAchievementDetails
            {
                AchievementId = achievementId,
                Description = achievement.Description,
                IconClass = achievement.IconClass,
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
                  var intermediateAnonymousResult = _dataContext.GetQueryable<GameDefinition>()
                    .Where(x => relatedEntityIds.Contains(x.Id))
                    .Select(x => new
                    {
                        AchievementRelatedGameDefinitionSummary = new AchievementRelatedGameDefinitionSummary
                        {
                            Id = x.Id,
                                Name = x.Name,
                                GamingGroupId = x.GamingGroupId
                        },
                        x.BoardGameGeekGameDefinitionId
                    } )
                    .OrderBy(x => x.AchievementRelatedGameDefinitionSummary.Name)
                    .ToList();

                    foreach (var recordWithABoardGameGeekGameDefinitionId in intermediateAnonymousResult.Where(x => x.BoardGameGeekGameDefinitionId != null))
                    {
                        recordWithABoardGameGeekGameDefinitionId.AchievementRelatedGameDefinitionSummary.BoardGameGeekInfo =
                            _boardGameGeekGameDefinitionInfoRetriever.GetResults(recordWithABoardGameGeekGameDefinitionId.BoardGameGeekGameDefinitionId.Value);
                    }
                    result.RelatedGameDefinitions = intermediateAnonymousResult.Select(x => x.AchievementRelatedGameDefinitionSummary).ToList();
                    break;
                case AchievementGroup.PlayedGame:
                    result.RelatedPlayedGames = _dataContext.GetQueryable<PlayedGame>()
                        .Where(y => relatedEntityIds.Contains(y.Id))
                        .Select(x => new AchievementRelatedPlayedGameSummary
                        {
                            GameDefinitionId = x.GameDefinitionId,
                            WinnerType = x.WinnerType,
                            DatePlayed = x.DatePlayed,
                            BoardGameGeekGameDefinitionId = x.GameDefinition.BoardGameGeekGameDefinitionId,
                            GameDefinitionName = x.GameDefinition.Name,
                            PlayedGameId = x.Id,
                            ThumbnailImageUrl = x.GameDefinition.BoardGameGeekGameDefinition.Thumbnail,
                            WinningPlayerName = x.PlayerGameResults.FirstOrDefault(y => y.GameRank == 1).Player.Name,
                            WinningPlayerId = x.PlayerGameResults.FirstOrDefault(y => y.GameRank == 1).PlayerId
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

        public virtual PlayerAchievementDetails GetPlayerAchievement(PlayerAchievementQuery playerAchievementQuery)
        {
            var achievementId = playerAchievementQuery.AchievementId;
            var achievement = _achievementRetriever.GetAchievement(achievementId);

            var result = new PlayerAchievementDetails
            {
                AchievementId = achievementId,
                Description = achievement.Description,
                IconClass = achievement.IconClass,
                AchievementName = achievement.Name,
                LevelThresholds = achievement.LevelThresholds
            };

            result.NumberOfPlayersWithThisAchievement = _dataContext.GetQueryable<PlayerAchievement>().Count(y => y.AchievementId == achievementId);


            Player player;
            if (playerAchievementQuery.PlayerId.HasValue)
            {
                player = _dataContext.FindById<Player>(playerAchievementQuery.PlayerId);
            }
            else
            {
                player = _playerRetriever.GetPlayerForCurrentUser(playerAchievementQuery.ApplicationUserId, playerAchievementQuery.GamingGroupId.Value);
            }

            if (player != null)
            {
                result.PlayerId = player.Id;
                result.PlayerName = player.Name;

                var achievementAwarded = achievement.IsAwardedForThisPlayer(player.Id);

                result.AchievementLevel = achievementAwarded.LevelAwarded;
                result.PlayerProgress = achievementAwarded.PlayerProgress;

                SetRelatedEntities(achievement.Group, result, achievementAwarded.RelatedEntities);
            }

            var playerAchievement = _dataContext
                .GetQueryable<PlayerAchievement>()
                .FirstOrDefault(x => x.AchievementId == achievementId && x.PlayerId == playerAchievementQuery.PlayerId);

            if (playerAchievement == null)
            {
                return result;
            }

            result.DateCreated = playerAchievement.DateCreated;
            result.LastUpdatedDate = playerAchievement.LastUpdatedDate;

            return result;
        }
    }
}
