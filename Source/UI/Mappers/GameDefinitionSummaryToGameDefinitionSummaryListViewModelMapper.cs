using AutoMapper;
using BusinessLogic.Models.Games;
using UI.Models.GameDefinitionModels;

namespace UI.Mappers
{
    public class GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper : BaseMapperService<GameDefinitionSummary, GameDefinitionSummaryListViewModel>
    {
        static GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper()
        {
            Mapper.CreateMap<GameDefinitionSummary, GameDefinitionSummaryListViewModel>();
        }
    }
}