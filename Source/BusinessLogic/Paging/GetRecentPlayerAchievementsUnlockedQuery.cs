namespace BusinessLogic.Paging
{
    public class GetRecentPlayerAchievementsUnlockedQuery : PagedQuery
    {
        public override string ToString()
        {
            return $"{this.GetType()}-{this.Page}-{this.PageSize}";

        }
    }
}