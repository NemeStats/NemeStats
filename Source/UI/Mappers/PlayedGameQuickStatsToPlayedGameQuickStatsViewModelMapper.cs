using AutoMapper;
using BusinessLogic.Models.PlayedGames;
using UI.Models.PlayedGame;

namespace UI.Mappers
{
    public class PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper : BaseMapperService<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>
    {
        static PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper()
        {
            Mapper.CreateMap<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>();
        }
    }
}