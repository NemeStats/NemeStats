namespace BusinessLogic.Models.Players
{
    public class CreatePlayerRequest
    {
        public string Name { get; set; }
        public int? GamingGroupId { get; set; }
        public string PlayerEmailAddress { get; set; }
    }
}
