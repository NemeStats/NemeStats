using System.Linq;
using AutoMapper;
using BusinessLogic.Models.Games;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;

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

    public class CreatePlayedGameRequestToNewlyCompletedGameMapper :
        BaseMapperService<CreatePlayedGameRequest, NewlyCompletedGame>
    {
        private readonly CreatePlayerRankRequestToPlayerRankMapper _createPlayerRankRequestToPlayerRankMapper;

        static CreatePlayedGameRequestToNewlyCompletedGameMapper()
        {
            Mapper.CreateMap<CreatePlayedGameRequest, NewlyCompletedGame>()
                .ForMember(m => m.PlayerRanks, o => o.Ignore());
        }

        public CreatePlayedGameRequestToNewlyCompletedGameMapper(CreatePlayerRankRequestToPlayerRankMapper createPlayerRankRequestToPlayerRankMapper)
        {
            _createPlayerRankRequestToPlayerRankMapper = createPlayerRankRequestToPlayerRankMapper;
        }

        public override NewlyCompletedGame Map(CreatePlayedGameRequest source)
        {
            var result = base.Map(source);

            result.PlayerRanks = _createPlayerRankRequestToPlayerRankMapper.Map(source.PlayerRanks).ToList();

            return result;
        }
    }

    public class CreatePlayerRankRequestToPlayerRankMapper :
       BaseMapperService<CreatePlayerRankRequest, PlayerRank>
    {
        static CreatePlayerRankRequestToPlayerRankMapper()
        {
            Mapper.CreateMap<CreatePlayerRankRequest, PlayerRank>();
        }
    }
}