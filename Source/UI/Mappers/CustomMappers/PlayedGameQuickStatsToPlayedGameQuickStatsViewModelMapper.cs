using AutoMapper;
using BusinessLogic.Models.PlayedGames;
using UI.Models.PlayedGame;

namespace UI.Mappers.CustomMappers
{
    public class PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper : BaseCustomMapper<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>
    {
        static PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper()
        {
            Mapper.CreateMap<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>();
        }
    }
}