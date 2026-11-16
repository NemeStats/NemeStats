using System.Linq;
using BusinessLogic.Models.Games;
using UI.Mappers.Interfaces;

namespace UI.Mappers.CustomMappers
{
    public class SavePlayedGameRequestToUpdatedGameGameMapper : BaseCustomMapper<SavePlayedGameRequest, UpdatedGame>
    {
        private readonly CreatePlayerRankRequestToPlayerRankMapper _createPlayerRankRequestToPlayerRankMapper;

        public SavePlayedGameRequestToUpdatedGameGameMapper(CreatePlayerRankRequestToPlayerRankMapper createPlayerRankRequestToPlayerRankMapper)
        {
            _createPlayerRankRequestToPlayerRankMapper = createPlayerRankRequestToPlayerRankMapper;
        }

        public override UpdatedGame Map(SavePlayedGameRequest source)
        {
            var result = base.Map(source);

            result.PlayerRanks = _createPlayerRankRequestToPlayerRankMapper.Map(source.PlayerRanks).ToList();

            return result;
        }
    }
}