using System;

namespace BusinessLogic.Events.Interfaces
{
    public interface IBusinessLogicEvent
    {
        int TriggerEntityId { get; set; }
    }
}