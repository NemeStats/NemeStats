namespace UI.Models.Points
{
    public class NemePointsSummaryViewModel
    {
        public NemePointsSummaryViewModel(int basePoints, int gameDurationBonusPoints, int gameWeightBonusPoints)
        {
            BaseNemePoints = basePoints;
            GameDurationBonusNemePoints = gameDurationBonusPoints;
            WeightBonusNemePoints = gameWeightBonusPoints;
            TotalNemePoints = basePoints + gameDurationBonusPoints + gameWeightBonusPoints;
        }

        public int TotalNemePoints { get; }
        public int BaseNemePoints { get; }
        public int WeightBonusNemePoints { get; }
        public int GameDurationBonusNemePoints { get; }

        protected bool Equals(NemePointsSummaryViewModel other)
        {
            return BaseNemePoints == other.BaseNemePoints
                && WeightBonusNemePoints == other.WeightBonusNemePoints
                && GameDurationBonusNemePoints == other.GameDurationBonusNemePoints;
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
            return Equals((NemePointsSummaryViewModel)obj);
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