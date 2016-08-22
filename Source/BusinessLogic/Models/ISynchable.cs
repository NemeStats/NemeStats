namespace BusinessLogic.Models
{
    public interface ISynchable
    {
        string ExternalSourceApplicationName { get; set; }
        string ExternalSourceEntityId { get; set; }
    }
}