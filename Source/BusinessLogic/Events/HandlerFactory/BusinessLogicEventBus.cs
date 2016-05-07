using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events.HandlerFactory
{
    public class BusinessLogicEventBus : IBusinessLogicEventBus
    {
        private readonly BusinessLogicEventsHandlerFactory _handlerFactory;

        public BusinessLogicEventBus(BusinessLogicEventsHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        public void SendEvent(IBusinessLogicEvent @event)
        {
            var eventHandlers = this._handlerFactory.GetHandlers(@event.GetType());
            foreach (var handlerInstance in eventHandlers)
            {
                try
                {
                    handlerInstance.Handle(@event);
                }
                catch (System.Exception ex)
                {
                    //this.Log().Error("Error handling event " + @event.GetType() + " with handler " + handlerInstance.HandlerMetaInfo.Type, ex);
                }
            }
        }
    }
}