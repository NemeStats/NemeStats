using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events.HandlerFactory
{
    public abstract class BusinessLogicEventSender: IBusinessLogicEventSender
    {
        private readonly IBusinessLogicEventBus _eventBus;

        protected BusinessLogicEventSender(IBusinessLogicEventBus eventBus)
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

    internal interface IBusinessLogicEventSender
    {
        void SendEvents(IList<IBusinessLogicEvent> events);
    }
}
