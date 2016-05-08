namespace BusinessLogic.Events.Interfaces
{
    public interface IBusinessLogicEventBus
    {
        void SendEvent(IBusinessLogicEvent @event);
    }
}