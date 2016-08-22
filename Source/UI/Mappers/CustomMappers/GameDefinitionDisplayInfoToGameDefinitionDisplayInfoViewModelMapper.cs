using System.Linq;
using AutoMapper;
using BusinessLogic.Models.Games;
using UI.Models.GameDefinitionModels;

namespace UI.Mappers.CustomMappers
{
    public class GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper :
        BaseCustomMapper<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>
    {
        static GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper()
        {
            Mapper.CreateMap<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>();
        }
    }

    public class CreatePlayedGameRequestToNewlyCompletedGameMapper :
        BaseCustomMapper<SavePlayedGameRequest, NewlyCompletedGame>
    {
        private readonly CreatePlayerRankRequestToPlayerRankMapper _createPlayerRankRequestToPlayerRankMapper;

        static CreatePlayedGameRequestToNewlyCompletedGameMapper()
        {
            Mapper.CreateMap<SavePlayedGameRequest, NewlyCompletedGame>()
                .ForMember(m => m.PlayerRanks, o => o.Ignore())
                .ForMember(m => m.GamingGroupId, o => o.Ignore())
                .ForMember(m => m.ExternalSourceApplicationName, o => o.Ignore())
                .ForMember(m => m.ExternalSourceEntityId, o => o.Ignore());
        }

        public CreatePlayedGameRequestToNewlyCompletedGameMapper(CreatePlayerRankRequestToPlayerRankMapper createPlayerRankRequestToPlayerRankMapper)
        {
            _createPlayerRankRequestToPlayerRankMapper = createPlayerRankRequestToPlayerRankMapper;
        }

        public override NewlyCompletedGame Map(SavePlayedGameRequest source)
        {
            var result = base.Map(source);

            result.PlayerRanks = _createPlayerRankRequestToPlayerRankMapper.Map(source.PlayerRanks).ToList();

            return result;
        }
    }

    public class CreatePlayerRankRequestToPlayerRankMapper :
       BaseCustomMapper<CreatePlayerRankRequest, PlayerRank>
    {
        static CreatePlayerRankRequestToPlayerRankMapper()
        {
            Mapper.CreateMap<CreatePlayerRankRequest, PlayerRank>();
        }
    }
}