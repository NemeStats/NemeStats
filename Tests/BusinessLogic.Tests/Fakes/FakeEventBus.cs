using System.Collections.Generic;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Tests.Fakes
{
    public class FakeEventBus : IBusinessLogicEventBus
    {
        public IList<IBusinessLogicEvent> EventsSended = new List<IBusinessLogicEvent>();
        public void SendEvent(IBusinessLogicEvent @event)
        {
            EventsSended.Add(@event);
        }
    }
}