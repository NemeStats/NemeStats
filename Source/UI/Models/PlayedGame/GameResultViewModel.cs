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
using System;
using System.Linq;
using UI.Models.Points;

namespace UI.Models.PlayedGame
{
    public class GameResultViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GameRank { get; set; }
        private string _gameRankString;
        public string GameRankString
        {
            get
            {
                if (_gameRankString != null)
                {
                    return _gameRankString;
                }

                var ones = GameRank % 10;
                var tens = Math.Floor((decimal)GameRank / 10) % 10;
                if (tens == 1)
                {
                    _gameRankString = $"{GameRank}th";
                }
                else
                {
                    switch (ones)
                    {
                        case 1:
                            _gameRankString = $"{GameRank}st";
                            break;
                        case 2:
                            _gameRankString = $"{GameRank}nd";
                            break;
                        case 3:
                            _gameRankString = $"{GameRank}rd";
                            break;
                        default:
                            _gameRankString = $"{GameRank}th";
                            break;
                    }
                }

                return _gameRankString;
            }
        }
        public int PlayedGameId { get; set; }
        public DateTime DatePlayed { get; set; }
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public Uri BoardGameGeekUri { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public bool PlayerActive { get; set; }
        public NemePointsSummaryViewModel NemePointsSummary { get; set; }
    }
}
