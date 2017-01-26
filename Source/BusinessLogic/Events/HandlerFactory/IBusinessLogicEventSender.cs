using System.Collections.Generic;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events.HandlerFactory
{
    public interface IBusinessLogicEventSender
    {
        void SendEvents(IList<IBusinessLogicEvent> events);
    }
}