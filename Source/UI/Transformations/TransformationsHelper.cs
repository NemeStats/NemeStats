using System.Collections.Generic;
using BusinessLogic.Models.Players;
using UI.Models.Badges;

namespace UI.Transformations
{
    public static class TransformationsHelper
    {
        public static List<IBadgeBaseViewModel> MapSpecialBadges(this PlayerWinRecord playerWinRecord)
        {
            var result = new List<IBadgeBaseViewModel>();

            if (playerWinRecord.IsChampion)
            {
                result.Add(new ChampionBadgeViewModel());
            }
            if (playerWinRecord.IsFormerChampion)
            {
                result.Add(new FormerChampionBadgeViewModel());
            }

            return result;

           
        }
    }
}