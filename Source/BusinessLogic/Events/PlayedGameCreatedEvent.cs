using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events
{
    public class PlayedGameCreatedEvent : IBusinessLogicEvent
    {
        public int TriggerEntityId { get; set; }
    }
}
