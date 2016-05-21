using System;
using System.Linq;
using AutoMapper;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using UI.Models.Achievements;

namespace UI.Mappers
{
    public class PlayerAchievementToPlayerAchievementViewModelMapper : BaseMapperService<PlayerAchievement, PlayerAchievementViewModel>
    {
        static PlayerAchievementToPlayerAchievementViewModelMapper()
        {
            Mapper.CreateMap<PlayerAchievement, PlayerAchievementViewModel>()
                .ForMember(m => m.RelatedGameDefintions, o => o.Ignore())
                .ForMember(m => m.Achievement, o => o.Ignore())
                .ForMember(m => m.PlayerProgress, o => o.Ignore());
        }

        private readonly AchievementToAchievementViewModelMapper _achievementToAchievementViewModelMapper;
        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper _gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper;
        private readonly PlayedGameRetriever _playedGameRetriever;
        private readonly PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper _playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper;

        public PlayerAchievementToPlayerAchievementViewModelMapper(
            AchievementToAchievementViewModelMapper achievementToAchievementViewModelMapper,
            IGameDefinitionRetriever gameDefinitionRetriever,
            GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper,
            PlayedGameRetriever playedGameRetriever,
            PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper
            )
        {
            _achievementToAchievementViewModelMapper = achievementToAchievementViewModelMapper;
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper = gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper;
            _playedGameRetriever = playedGameRetriever;
            _playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper = playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper;
        }

        public override PlayerAchievementViewModel Map(PlayerAchievement source)
        {
            var result = base.Map(source);

            var achievement = AchievementFactory.GetAchievementById(source.AchievementId);
            result.Achievement = _achievementToAchievementViewModelMapper.Map(achievement);
            result.PlayerProgress = achievement.IsAwardedForThisPlayer(result.PlayerId).PlayerProgress;

            switch (achievement.Group)
            {
                case AchievementGroup.Game:
                    result.RelatedGameDefintions =
                        _gameDefinitionRetriever.GetGameDefinitionSummaries(source.RelatedEntities)
                            .Select(g => _gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper.Map(g))
                            .ToList();
                    break;
                case AchievementGroup.Player:
                    break;
                case AchievementGroup.PlayedGame:
                    result.RelatedPlayedGames =
                       _playedGameRetriever.GetPlayedGamesQuickStats(source.RelatedEntities)
                           .Select(g => _playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper.Map(g))
                           .ToList();
                    break;

            }

            return result;
        }
    }
    
    
}