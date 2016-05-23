using AutoMapper;
using BusinessLogic.Models;
using UI.Models.Players;

namespace UI.Mappers
{
    public class PlayerToPlayerListSummaryViewModelMapper : BaseMapperService<Player, PlayerListSummaryViewModel>
    {
        static PlayerToPlayerListSummaryViewModelMapper()
        {
            Mapper.CreateMap<Player, PlayerListSummaryViewModel>()
                .ForMember(m => m.PlayerName, o => o.MapFrom(s => s.Name))
                .ForMember(m => m.PlayerId, o => o.MapFrom(s => s.Id))
                .ForMember(m => m.GamingGroupId, o => o.MapFrom(s => s.GamingGroupId))
                .ForMember(m => m.GamingGroupName, o => o.MapFrom(s => s.GamingGroup.Name));
        }
    }
}