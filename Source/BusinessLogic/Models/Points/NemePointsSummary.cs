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

        protected bool Equals(NemePointsSummary other)
        {
            return BaseNemePoints == other.BaseNemePoints && WeightBonusNemePoints == other.WeightBonusNemePoints && GameDurationBonusNemePoints == other.GameDurationBonusNemePoints;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((NemePointsSummary)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = BaseNemePoints;
                hashCode = (hashCode * 397) ^ WeightBonusNemePoints;
                hashCode = (hashCode * 397) ^ GameDurationBonusNemePoints;
                return hashCode;
            }
        }
    }
}
