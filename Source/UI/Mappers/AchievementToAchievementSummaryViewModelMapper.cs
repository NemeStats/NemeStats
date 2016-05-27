using BusinessLogic.Logic.Achievements;
using UI.Models.Achievements;

namespace UI.Mappers
{
    public class AchievementToAchievementSummaryViewModelMapper : BaseMapperService<IAchievement , AchievementSummaryViewModel>
    {
        static AchievementToAchievementSummaryViewModelMapper()
        {
            AutoMapper.Mapper.CreateMap<IAchievement, AchievementSummaryViewModel>();
        }
    }
}