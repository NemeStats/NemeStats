using System.Data.Entity;
using System.Linq;
using AutoMapper;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using UI.Models.Achievements;

namespace UI.Mappers
{
    public class AchievementToAchievementViewModelMapper : BaseMapperService<IAchievement, AchievementViewModel>
    {
        
        private readonly PlayerAchievementToPlayerAchievementWinnerViewModelMapper _achievementToPlayerAchievementWinnerViewModelMapper;

        public AchievementToAchievementViewModelMapper(PlayerAchievementToPlayerAchievementWinnerViewModelMapper achievementToPlayerAchievementWinnerViewModelMapper)
        {
            
            _achievementToPlayerAchievementWinnerViewModelMapper = achievementToPlayerAchievementWinnerViewModelMapper;
        }

        static AchievementToAchievementViewModelMapper()
        {
            Mapper.CreateMap<IAchievement, AchievementViewModel>()
                .ForMember(m => m.Winners, o => o.Ignore());
        }

        public override AchievementViewModel Map(IAchievement source)
        {
            var model = base.Map(source);

            //We have to use the same context than the achievement to avoid errors. TODO: Fix the scope error
            var winners = source.DataContext.GetQueryable<PlayerAchievement>().Where(a => a.AchievementId == source.Id)
                .Include(a => a.Player)
                .Include(a => a.Player.GamingGroup)
                .ToList();
            model.Winners = winners.Select(w=> _achievementToPlayerAchievementWinnerViewModelMapper.Map(w)).ToList();

            return model;
        }
    }
}