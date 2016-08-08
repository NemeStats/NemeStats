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
        /// <returns></returns>
        public static void LinkOrphanGames([TimerTrigger("0 0 4 * * *")] TimerInfo info)
        {
            var linkOrphanGamesResult = Program.Container.GetInstance<IBoardGameGeekBatchUpdateService>().LinkOrphanGames();
            Console.WriteLine(linkOrphanGamesResult);
        }
    }
}
