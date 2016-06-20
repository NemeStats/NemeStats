using AutoMapper;
using BusinessLogic.Models.Games;
using UI.Models.GameDefinitionModels;

namespace UI.Mappers
{
    public class GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper :
        BaseMapperService<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>
    {
        static GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper()
        {
            Mapper.CreateMap<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>();
        }
    }
}