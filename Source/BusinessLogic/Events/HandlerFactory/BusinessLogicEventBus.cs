using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events.HandlerFactory
{
    public class BusinessLogicEventBus : IBusinessLogicEventBus
    {
        private readonly BusinenessLogicEventsHandlerFactory _handlerFactory;

        public BusinessLogicEventBus(BusinenessLogicEventsHandlerFactory handlerFactory)
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