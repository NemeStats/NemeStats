using System;
using System.Collections.Generic;
using BusinessLogic.Events.HandlerFactory;

namespace BusinessLogic.Events.Interfaces
{
    public interface IHandlerFactory
    {
        IList<HandlerInstance> GetHandlers(Type messageType);
    }
}