using BusinessLogic.Logic.Achievements;
using UI.Models.Achievements;

namespace UI.Mappers.CustomMappers
{
    public class AchievementToAchievementSummaryViewModelMapper : BaseCustomMapper<IAchievement , AchievementSummaryViewModel>
    {
        static AchievementToAchievementSummaryViewModelMapper()
        {
            AutoMapper.Mapper.CreateMap<IAchievement, AchievementSummaryViewModel>();
        }
    }
}