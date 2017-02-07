using System;
using System.Linq;
using AutoMapper;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using UI.Models.Achievements;

namespace UI.Mappers.CustomMappers
{
    public class PlayerAchievementToPlayerAchievementViewModelMapper : BaseCustomMapper<PlayerAchievement, PlayerAchievementViewModel>
    {
        static PlayerAchievementToPlayerAchievementViewModelMapper()
        {
            Mapper.CreateMap<PlayerAchievement, PlayerAchievementViewModel>()
                .ForMember(m => m.RelatedGameDefinitions, o => o.Ignore())
                .ForMember(m => m.AchievementTile, o => o.Ignore())
                .ForMember(m => m.PlayerProgress, o => o.Ignore());
        }

        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper _gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper;
        private readonly PlayedGameRetriever _playedGameRetriever;
        private readonly PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper _playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper;
        private readonly IPlayerRetriever _playerRetriever;
        private readonly PlayerToPlayerListSummaryViewModelMapper _playerToPlayerListSummaryViewModelMapper;
        private readonly ITransformer _transformer;
        private readonly IAchievementRetriever _achievementRetriever;

        public PlayerAchievementToPlayerAchievementViewModelMapper(
            IGameDefinitionRetriever gameDefinitionRetriever,
            GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper,
            PlayedGameRetriever playedGameRetriever,
            PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper,
            IPlayerRetriever playerRetriever,
            PlayerToPlayerListSummaryViewModelMapper playerToPlayerListSummaryViewModelMapper, 
            ITransformer transformer, 
            IAchievementRetriever achievementRetriever)
        {
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper = gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper;
            _playedGameRetriever = playedGameRetriever;
            _playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper = playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper;
            _playerRetriever = playerRetriever;
            _playerToPlayerListSummaryViewModelMapper = playerToPlayerListSummaryViewModelMapper;
            _transformer = transformer;
            _achievementRetriever = achievementRetriever;
        }

        [Obsolete("THIS IS A REMINDER TO COME BACK AND FIX THIS SOON")]

        public override PlayerAchievementViewModel Map(PlayerAchievement source)
        {
            var result = base.Map(source);

            //var achievementSummary = _achievementRetriever.GetAchievementSummary

            //result.AchievementTile = _achievementToAchievementViewModelMapper.Map(achievement);
            //result.PlayerProgress = achievement.IsAwardedForThisPlayer(result.PlayerId).PlayerProgress;

            //switch (achievementSummary.Group)
            //{
            //    case AchievementGroup.Game:
            //        result.RelatedGameDefinitions =
            //            _gameDefinitionRetriever.GetGameDefinitionSummaries(source.RelatedEntities)
            //                .Select(g => _gameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper.Map(g))
            //                .ToList();
            //        break;
            //    case AchievementGroup.Player:
            //        result.RelatedPlayers =
            //           _playerRetriever.GetPlayers(source.RelatedEntities)
            //               .Select(g => _playerToPlayerListSummaryViewModelMapper.Map(g))
            //               .ToList();
            //        break;
            //    case AchievementGroup.PlayedGame:
            //        result.RelatedPlayedGames =
            //           _playedGameRetriever.GetPlayedGamesQuickStats(source.RelatedEntities)
            //               .Select(g => _playedGameQuickStatsToPlayedGameQuickStatsViewModelMapper.Map(g))
            //               .ToList();
            //        break;

            //}

            return result;
        }
    }
    
    
}