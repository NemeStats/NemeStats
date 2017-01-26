using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events.HandlerFactory
{
    public class BusinessLogicEventSender: IBusinessLogicEventSender
    {
        private readonly IBusinessLogicEventBus _eventBus;

        public BusinessLogicEventSender(IBusinessLogicEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public virtual void SendEvents(IList<IBusinessLogicEvent> events)
        {
            if (events.Any())
            {
                foreach (var @event in events)
                {
                    _eventBus.SendEvent(@event);
                }
            }
        }
    }
}
