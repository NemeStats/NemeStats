using AutoMapper;
using BusinessLogic.Models;
using UI.Models.Players;

namespace UI.Mappers
{
    public class PlayerAchievementToPlayerAchievementWinnerViewModelMapper :
        BaseMapperService<PlayerAchievement, PlayerAchievementWinnerViewModel>
    {
        static PlayerAchievementToPlayerAchievementWinnerViewModelMapper()
        {
            Mapper.CreateMap<PlayerAchievement, PlayerAchievementWinnerViewModel>()
                .ForMember(m => m.PlayerName, o => o.MapFrom(p => p.Player.Name))
                .ForMember(m => m.PlayerId, o => o.MapFrom(p => p.PlayerId))
                .ForMember(m => m.GamingGroupId, o => o.MapFrom(p => p.Player.GamingGroupId))
                .ForMember(m => m.GamingGroupName, o => o.MapFrom(p => p.Player.GamingGroup.Name))
                .ForMember(m => m.AchievementLevel, o => o.MapFrom(p => p.AchievementLevel))
                .ForMember(m => m.AchievementLastUpdateDate, o => o.MapFrom(p => p.LastUpdatedDate));
        }
    }
}