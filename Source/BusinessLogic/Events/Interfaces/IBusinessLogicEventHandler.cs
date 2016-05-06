namespace BusinessLogic.Events.Interfaces
{
    public interface IBusinessLogicEventHandler<in T>
        where T : IBusinessLogicEvent
    {
        void Handle(T @event);
    }
}