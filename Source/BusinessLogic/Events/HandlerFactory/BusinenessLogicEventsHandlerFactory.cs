using System;
using System.Collections.Generic;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events.HandlerFactory
{
    public class BusinenessLogicEventsHandlerFactory : HandlerFactory, IHandlerFactory
    {
        public BusinenessLogicEventsHandlerFactory(HandlerFactoryConfiguration factoryConfiguration)
            : base(factoryConfiguration, typeof(IBusinessLogicEventHandler<>), typeof(IBusinessLogicEvent))
        {
        }

        public IList<HandlerInstance> GetHandlers(Type messageType)
        {
            return base.GetHandlers(messageType);
        }

    }
}
