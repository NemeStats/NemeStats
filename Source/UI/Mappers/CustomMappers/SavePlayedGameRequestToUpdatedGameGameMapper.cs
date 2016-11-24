using System.Linq;
using AutoMapper;
using BusinessLogic.Models.Games;
using UI.Mappers.Interfaces;

namespace UI.Mappers.CustomMappers
{
    public class SavePlayedGameRequestToUpdatedGameGameMapper : BaseCustomMapper<SavePlayedGameRequest, UpdatedGame>
    {
        private readonly CreatePlayerRankRequestToPlayerRankMapper _createPlayerRankRequestToPlayerRankMapper;

        static SavePlayedGameRequestToUpdatedGameGameMapper()
        {
            Mapper.CreateMap<SavePlayedGameRequest, UpdatedGame>()
                .ForMember(m => m.PlayerRanks, o => o.Ignore())
                .ForMember(m => m.ApplicationLinkages, opt => opt.Ignore())
                .ForMember(m => m.GamingGroupId, opt => opt.Ignore());
        }

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