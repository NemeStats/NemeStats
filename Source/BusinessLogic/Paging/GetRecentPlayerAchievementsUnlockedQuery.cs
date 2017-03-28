namespace BusinessLogic.Paging
{
    public class GetRecentPlayerAchievementsUnlockedQuery : PagedQuery
    {
        public int? PlayerId { get; set; }

        protected bool Equals(GetRecentPlayerAchievementsUnlockedQuery other)
        {
            return base.Equals(other) && PlayerId == other.PlayerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GetRecentPlayerAchievementsUnlockedQuery)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ PlayerId.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{GetType()}-{Page}-{PageSize}";

        }
    }
}