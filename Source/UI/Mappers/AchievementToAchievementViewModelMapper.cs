using AutoMapper;
using BusinessLogic.Logic.Achievements;
using UI.Models.Achievements;

namespace UI.Mappers
{
    public class AchievementToAchievementViewModelMapper : BaseMapperService<IAchievement, AchievementViewModel>
    {
        static AchievementToAchievementViewModelMapper()
        {
            Mapper.CreateMap<IAchievement, AchievementViewModel>();
        }
    }
}