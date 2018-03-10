namespace BusinessLogic.Models.Nemeses
{
    public class GetRecentNemesisChangesRequest
    {
        public int NumberOfRecentChangesToRetrieve { get; set; }
        public int? GamingGroupId { get; set; }
    }
}
