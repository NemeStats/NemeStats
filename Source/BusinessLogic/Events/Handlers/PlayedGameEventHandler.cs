using System;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using Exceptionless;
using RollbarSharp;

namespace BusinessLogic.Events.Handlers
{
    public class PlayedGameEventHandler : BaseEventHandler, IBusinessLogicEventHandler<PlayedGameCreatedEvent>
    {
        private readonly INemeStatsEventTracker _playedGameEventTracker;
        private readonly INemesisRecalculator _nemesisRecalculator;
        private readonly IGamingGroupChampionRecalculator _gamingGroupChampionRecalculator;
        private readonly IChampionRecalculator _championRecalculator;
        private readonly IAchievementProcessor _achievementProcessor;
        private readonly IRollbarClient _rollbar;

        public PlayedGameEventHandler(
            IDataContext dataContext,
            INemeStatsEventTracker playedGameEventTracker, 
            IAchievementProcessor achievementProcessor, 
            IChampionRecalculator championRecalculator, 
            INemesisRecalculator nemesisRecalculator, 
            IGamingGroupChampionRecalculator gamingGroupChampionRecalculator,
            IRollbarClient rollbar) : base(dataContext)
        {
            _playedGameEventTracker = playedGameEventTracker;
            _achievementProcessor = achievementProcessor;
            _championRecalculator = championRecalculator;
            _nemesisRecalculator = nemesisRecalculator;
            _gamingGroupChampionRecalculator = gamingGroupChampionRecalculator;
            _rollbar = rollbar;
        }

        private static readonly object ChampionLock = new object();
        private static readonly object NemesisLock = new object();
        private static readonly object AchievementsLock = new object();
        private static readonly object GamingGroupChampionLock = new object();

        public bool Handle(PlayedGameCreatedEvent @event)
        {
            //--process analytics
            try
            {
                _playedGameEventTracker.TrackPlayedGame(@event.CurrentUser, @event.TransactionSource);
            }
            catch (Exception ex)
            {
                _rollbar.SendException(ex);
                ex.ToExceptionless();
            }

            bool noExceptions;
            //--this is a weak solution to duplicate key exceptions getting logged when multiple games are recorded in quick succession. A better solution
            //  would be to only lock at the level required instead of locking globally
            lock (ChampionLock)
            {
                //--process champions
                //TODO this should only lock for each distinct GameDefinitionId
                _championRecalculator.RecalculateChampion(@event.GameDefinitionId, @event.CurrentUser, DataContext);
            }

            lock (NemesisLock)
            {
                //--process nemeses
                //TODO this should only lock for each distinct playerId
                foreach (var playerId in @event.ParticipatingPlayerIds)
                {
                    _nemesisRecalculator.RecalculateNemesis(playerId, @event.CurrentUser, DataContext);
                }
            }

            lock (GamingGroupChampionLock)
            {
                _gamingGroupChampionRecalculator.RecalculateGamingGroupChampion(@event.TriggerEntityId);
            }

            lock (AchievementsLock)
            {
                //--process achievements
                //TODO this should probably only lock at the GamingGroupdId level
                noExceptions = _achievementProcessor.ProcessAchievements(@event.TriggerEntityId);
            }
            
            return noExceptions;
        }
    }
}
