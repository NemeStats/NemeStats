namespace UI.Areas.Api.Models
{
    public class UpdatedPlayedGameMessage : SaveablePlayedGameMessageBase
    {
        public int PlayedGameId { get; set; }
    }
}