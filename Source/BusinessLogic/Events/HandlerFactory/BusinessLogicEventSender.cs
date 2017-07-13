using System.Threading.Tasks;
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

        public virtual Task SendEvent(IBusinessLogicEvent businessLogicEvent)
        {
            Task task = null;
            if (businessLogicEvent != null)
            {
                task =  _eventBus.SendEvent(businessLogicEvent);
            }

            return task;
        }
    }
}
