namespace BusinessLogic.Paging
{
    public class GetRecentPlayerAchievementsUnlockedQuery : PagedQuery
    {
        public int? PlayerId { get; set; }
        public int? GamingGroupId { get; set; }
        public bool IncludeOnlyOnePage { get; set; }

        protected bool Equals(GetRecentPlayerAchievementsUnlockedQuery other)
        {
            return base.Equals(other) && PlayerId == other.PlayerId && GamingGroupId == other.GamingGroupId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GetRecentPlayerAchievementsUnlockedQuery) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ PlayerId.GetHashCode();
                hashCode = (hashCode * 397) ^ GamingGroupId.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{GetType()}-{Page}-{PageSize}";

        }
    }
}