using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Points
{
    public class NemePointsSummary
    {
        public NemePointsSummary()
        {
            
        }

        public NemePointsSummary(int baseNemePoints, int gameDurationBonusPoints, int weightBonusPoints)
        {
            BaseNemePoints = baseNemePoints;
            GameDurationBonusNemePoints = gameDurationBonusPoints;
            WeightBonusNemePoints = weightBonusPoints;
        }

        public int BaseNemePoints { get; set; }
        public int WeightBonusNemePoints { get; set; }
        public int GameDurationBonusNemePoints { get; set; }

        public int TotalPoints => BaseNemePoints + WeightBonusNemePoints + GameDurationBonusNemePoints;
    }
}
