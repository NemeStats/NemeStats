using System;
using System.Collections.Generic;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Events.HandlerFactory
{
    public class BusinessLogicEventsHandlerFactory : HandlerFactory, IHandlerFactory
    {
        public BusinessLogicEventsHandlerFactory(HandlerFactoryConfiguration factoryConfiguration)
            : base(factoryConfiguration, typeof(IBusinessLogicEventHandler<>), typeof(IBusinessLogicEvent))
        {
        }

        public IList<HandlerInstance> GetHandlers(Type messageType)
        {
            return base.GetHandlers(messageType);

        }

    }
}
