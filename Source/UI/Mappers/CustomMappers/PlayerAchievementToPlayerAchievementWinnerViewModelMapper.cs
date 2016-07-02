using System.Collections.Generic;
using AutoMapper;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Models;
using UI.Models.Players;

namespace UI.Mappers.CustomMappers
{
    public class PlayerAchievementToPlayerAchievementWinnerViewModelMapper :
        BaseCustomMapper<PlayerAchievement, PlayerAchievementWinnerViewModel>
    {
        private readonly AchievementToAchievementSummaryViewModelMapper _achievementSummaryViewModelMapper;

        public PlayerAchievementToPlayerAchievementWinnerViewModelMapper(AchievementToAchievementSummaryViewModelMapper achievementSummaryViewModelMapper)
        {
            _achievementSummaryViewModelMapper = achievementSummaryViewModelMapper;
        }

        static PlayerAchievementToPlayerAchievementWinnerViewModelMapper()
        {
            Mapper.CreateMap<PlayerAchievement, PlayerAchievementWinnerViewModel>()
                .ForMember(m => m.Achievement, o => o.Ignore())
                .ForMember(m => m.AchievementId, o => o.MapFrom(p => p.AchievementId))
                .ForMember(m => m.PlayerName, o => o.MapFrom(p => p.Player.Name))
                .ForMember(m => m.PlayerId, o => o.MapFrom(p => p.PlayerId))
                .ForMember(m => m.UserId, o => o.MapFrom(p => p.Player.ApplicationUserId))
                .ForMember(m => m.GamingGroupId, o => o.MapFrom(p => p.Player.GamingGroupId))
                .ForMember(m => m.GamingGroupName, o => o.MapFrom(p => p.Player.GamingGroup.Name))
                .ForMember(m => m.AchievementLevel, o => o.MapFrom(p => p.AchievementLevel))
                .ForMember(m => m.AchievementLastUpdateDate, o => o.MapFrom(p => p.LastUpdatedDate));

        }

        public override PlayerAchievementWinnerViewModel Map(PlayerAchievement source)
        {
            var model = base.Map(source);

            model.Achievement =
                _achievementSummaryViewModelMapper.Map(AchievementFactory.GetAchievementById(source.AchievementId));

            return model;
        }

        public override IEnumerable<PlayerAchievementWinnerViewModel> Map(IEnumerable<PlayerAchievement> source)
        {
            var model = base.Map(source);

            foreach (var viewmodel in model)
            {
                viewmodel.Achievement = _achievementSummaryViewModelMapper.Map(AchievementFactory.GetAchievementById(viewmodel.AchievementId));
            }

            

            return model;
        }
    }
}