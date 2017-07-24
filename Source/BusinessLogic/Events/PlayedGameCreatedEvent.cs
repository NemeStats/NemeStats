using System;
using System.Collections.Generic;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.Logic;
using BusinessLogic.Models.User;

namespace BusinessLogic.Events
{
    public class PlayedGameCreatedEvent : IBusinessLogicEvent
    {
        public PlayedGameCreatedEvent(int playedGameId, int gameDefinitionId, List<int> participatingPlayerIds, TransactionSource expectedTransactionSource, ApplicationUser currentUser)
        {
            TriggerEntityId = playedGameId;
            TransactionSource = expectedTransactionSource;
            GameDefinitionId = gameDefinitionId;
            ParticipatingPlayerIds = participatingPlayerIds;
            CurrentUser = currentUser;
        }

        public int TriggerEntityId { get; set; }
        public int GameDefinitionId { get; set; }
        public List<int> ParticipatingPlayerIds { get; set; }
        public TransactionSource TransactionSource { get; set; }
        public ApplicationUser CurrentUser { get; set; }

        public event Action EventHandled;
        public void TriggerEventHandledAction()
        {
            //--if this looks funky then read this: https://blogs.msdn.microsoft.com/ericlippert/2009/04/29/events-and-races/
            var temp = EventHandled;
            temp?.Invoke();
        }
    }
}
