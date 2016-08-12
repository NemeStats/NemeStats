using System;
using System.IO;
using BusinessLogic.Jobs.BoardGameGeekCleanUpService;
using Microsoft.Azure.WebJobs;

namespace NemeStats.ScheduledJobs
{
    public class Functions
    {
        /// <summary>
        ///     Every day at 04:00
        /// </summary>
        /// <param name="info"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [NoAutomaticTriggerAttribute]
        public static void LinkOrphanGames([TimerTrigger("0 0 4 * * *")] TimerInfo info, TextWriter log)
        {
            var linkOrphanGamesResult = Program.Container.GetInstance<IBoardGameGeekBatchUpdateService>().LinkOrphanGames();
            log.WriteLine(linkOrphanGamesResult);
        }
    }
}
