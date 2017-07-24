using System.Threading.Tasks;

namespace BusinessLogic.Events.Interfaces
{
    public interface IBusinessLogicEventBus
    {
        Task SendEvent(IBusinessLogicEvent @event);
    }
}