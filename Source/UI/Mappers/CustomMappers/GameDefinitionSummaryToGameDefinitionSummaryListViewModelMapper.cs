using AutoMapper;
using BusinessLogic.Models.Games;
using UI.Models.GameDefinitionModels;

namespace UI.Mappers.CustomMappers
{
    public class GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper : BaseCustomMapper<GameDefinitionSummary, GameDefinitionSummaryListViewModel>
    {
        static GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper()
        {
            Mapper.CreateMap<GameDefinitionSummary, GameDefinitionSummaryListViewModel>();
        }
    }
}