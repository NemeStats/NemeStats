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
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class PlayerGameResult : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }

        [Index("IX_PlayerId_and_PlayedGameId", 1, IsUnique = true)]
        public int PlayedGameId { get; set; }
        [Index("IX_PlayerId_and_PlayedGameId", 2, IsUnique = true)]
        public int PlayerId { get; set; }
        public int GameRank { get; set; }
        public int NemeStatsPointsAwarded { get; set; }
        public int GameWeightBonusPoints { get; set; }
        public int GameDurationBonusPoints { get; set; }
        public decimal? PointsScored { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int TotalPoints
        {
            get
            {
                return NemeStatsPointsAwarded + GameDurationBonusPoints + GameWeightBonusPoints;
            }
            private set { /* needed for EF */ }
        }

        public virtual PlayedGame PlayedGame { get; set; }
        public virtual Player Player { get; set; }
    }
}
