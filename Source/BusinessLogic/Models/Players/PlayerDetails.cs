#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using System.Collections.Generic;
using BusinessLogic.Models.Points;

namespace BusinessLogic.Models.Players
{
    public class PlayerDetails
    {
        public PlayerDetails()
        {
            CurrentNemesis = new NullNemesis();
            PreviousNemesis = new NullNemesis();
            this.PlayerVersusPlayersStatistics = new List<PlayerVersusPlayerStatistics>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ApplicationUserId { get; set; }
        public bool Active { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }

        public PlayerStatistics PlayerStats { get; set; }
        public IList<PlayerGameResult> PlayerGameResults { get; set; }
        public Nemesis CurrentNemesis { get; set; }
        public Nemesis PreviousNemesis { get; set; }
        public IList<Player> Minions { get; set; }
        public IList<PlayerGameSummary> PlayerGameSummaries { get; set; }
        public IList<Champion> ChampionedGames { get; set; }
        public IList<GameDefinition> FormerChampionedGames { get; set; }
        public IList<PlayerVersusPlayerStatistics> PlayerVersusPlayersStatistics { get; set; }
        public int LongestWinningStreak { get; set; }
        public NemePointsSummary NemePointsSummary { get; set; }
        public List<Achievement> Achievements { get; set; }
    }
}
