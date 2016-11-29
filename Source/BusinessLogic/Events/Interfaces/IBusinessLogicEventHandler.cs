namespace BusinessLogic.Events.Interfaces
{
    public interface IBusinessLogicEventHandler<in T>
        where T : IBusinessLogicEvent
    {
        bool Handle(T @event);
    }
}