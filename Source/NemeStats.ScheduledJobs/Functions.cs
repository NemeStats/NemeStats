using System.Diagnostics;
using System.IO;
using BusinessLogic.Jobs.BoardGameGeekBatchUpdateJobService;
using Microsoft.Azure.WebJobs;

namespace NemeStats.ScheduledJobs
{
    public class Functions
    {
        /// <summary>
        /// Every day at 04:00
        /// </summary>
        /// <param name="info"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [NoAutomaticTrigger]
        public static void LinkOrphanGames([TimerTrigger("0 0 4 * * *")] TimerInfo info, TextWriter log)
        {
            var jobResult = Program.Container.GetInstance<IBoardGameGeekBatchUpdateJobService>().LinkOrphanGames();
            log.WriteLine(jobResult);
        }

        /// <summary>
        /// Updates all the BGG data. Use with caution!
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        [NoAutomaticTrigger]
        public static void RefreshAllBoardGameGeekData(TextWriter log)
        {
            var clock = new Stopwatch();

            var boardGameGeekBatchUpdateJobService = Program.Container.GetInstance<IBoardGameGeekBatchUpdateJobService>();
            clock.Start();
            var jobResult = boardGameGeekBatchUpdateJobService.RefreshAllBoardGameGeekData();
            clock.Stop();

            log.WriteLine($"Updated {jobResult} games in {clock.Elapsed}");
        }

        /// <summary>
        /// Updates all the BGG data. Use with caution!
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        [NoAutomaticTrigger]
        public static void RefreshOutdatedBoardGameGeekData([TimerTrigger("0 0 6 * * *")] TimerInfo info, TextWriter log, int daysOutdated, int maxElementsToUpdate)
        {
            var clock = new Stopwatch();

            if (daysOutdated == 0)
            {
                daysOutdated = 7;
            }

            var boardGameGeekBatchUpdateJobService = Program.Container.GetInstance<IBoardGameGeekBatchUpdateJobService>();
            clock.Start();
            var jobResult = boardGameGeekBatchUpdateJobService.RefreshOutdatedBoardGameGeekData(daysOutdated, maxElementsToUpdate > 0 ? maxElementsToUpdate : (int?)null);
            clock.Stop();

            log.WriteLine($"Updated {jobResult} games in {clock.Elapsed}");
        }
    }
}
