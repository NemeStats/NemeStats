using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Jobs.BoardGameGeekBatchUpdateJobService
{
    public class LinkOrphanGamesJobResult : BaseJobResult
    {
        public LinkOrphanGamesJobResult()
        {
            LinkedGames = 0;
            StillOrphanGames = new List<OrphanGame>();
        }

        public int OrphanGames { get; set; }
        public int LinkedGames { get; set; }

        public List<OrphanGame> StillOrphanGames { get; set; }
        

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());
            sb.AppendLine($"Orphan games detected: {OrphanGames}");
            sb.AppendLine($"Linked games: {LinkedGames}");
            sb.AppendLine($"Still Orphan games: {StillOrphanGames.Count}");
            foreach (var orphanGame in StillOrphanGames)
            {
                sb.AppendLine($"{orphanGame.Name} - Id: {orphanGame.Id} - GamingGroupId - {orphanGame.GamingGroupId}");
            }

            return sb.ToString();
        }

        public class OrphanGame
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public int GamingGroupId { get; set; }
        }
    }
}