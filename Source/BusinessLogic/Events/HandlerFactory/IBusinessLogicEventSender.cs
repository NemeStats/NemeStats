using System.Threading.Tasks;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events.HandlerFactory
{
    public interface IBusinessLogicEventSender
    {
        Task SendEvent(IBusinessLogicEvent businessLogicEvent);
    }
}