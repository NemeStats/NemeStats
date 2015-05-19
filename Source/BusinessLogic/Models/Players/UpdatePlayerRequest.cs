namespace BusinessLogic.Models.Players
{
    public class UpdatePlayerRequest
    {
        public int PlayerId { get; set; }
        public bool? Active { get; set; }
        public string Name { get; set; }
    }
}