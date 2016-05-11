using System.Threading;
using System.Threading.Tasks;
using BusinessLogic.Events.Interfaces;
using RollbarSharp;

namespace BusinessLogic.Events.HandlerFactory
{
    public class BusinessLogicEventBus : IBusinessLogicEventBus
    {
        private readonly BusinessLogicEventsHandlerFactory _handlerFactory;
        private readonly IRollbarClient _rollbar;

        public BusinessLogicEventBus(BusinessLogicEventsHandlerFactory handlerFactory, IRollbarClient rollbar)
        {
            _handlerFactory = handlerFactory;
            _rollbar = rollbar;
        }

        public virtual void SendEvent(IBusinessLogicEvent @event)
        {

            Task.Factory.StartNew(() =>
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
                        _rollbar.SendException(ex);
                    }
                }                
            });

            
        }
    }
}