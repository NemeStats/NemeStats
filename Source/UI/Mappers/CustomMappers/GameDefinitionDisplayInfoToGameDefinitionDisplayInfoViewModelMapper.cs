using System.Linq;
using BusinessLogic.Models.Games;
using UI.Models.GameDefinitionModels;

namespace UI.Mappers.CustomMappers
{
    public class GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper :
        BaseCustomMapper<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>
    {
    }

    public class CreatePlayedGameRequestToNewlyCompletedGameMapper :
        BaseCustomMapper<SavePlayedGameRequest, NewlyCompletedGame>
    {
        private readonly CreatePlayerRankRequestToPlayerRankMapper _createPlayerRankRequestToPlayerRankMapper;

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
    }
}