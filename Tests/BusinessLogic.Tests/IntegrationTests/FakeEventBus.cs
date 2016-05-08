using System;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Tests.IntegrationTests
{
    public class FakeEventBus : IBusinessLogicEventBus
    {
        public void SendEvent(IBusinessLogicEvent @event)
        {
            
        }
    }
}