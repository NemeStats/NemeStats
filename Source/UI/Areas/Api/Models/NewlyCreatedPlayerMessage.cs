namespace UI.Areas.Api.Models
{
    public class NewlyCreatedPlayerMessage
    {
        public int PlayerId { get; set; }
        public int GamingGroupId { get; set; }
        public string NemeStatsUrl { get; set; }
    }
}