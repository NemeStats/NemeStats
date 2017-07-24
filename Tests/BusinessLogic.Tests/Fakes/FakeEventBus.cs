using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Tests.Fakes
{
    public class FakeEventBus : IBusinessLogicEventBus
    {
        public IList<IBusinessLogicEvent> EventsSended = new List<IBusinessLogicEvent>();
        public Task SendEvent(IBusinessLogicEvent @event)
        {
            EventsSended.Add(@event);
            return Task.Factory.StartNew(() => { });
        }
    }
}