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

namespace BusinessLogic.Exceptions
{
    public class PlayerAlreadyExistsException : DuplicateKeyException
    {
        internal const string EXCEPTION_MESSAGE = "A Player with name '{0}' already exists in this Gaming Group. The existing playerId is '{1}'.";

        public int ExistingPlayerId { get; set; }

        public PlayerAlreadyExistsException(string playerName, int existingPlayerId)
            : base(string.Format(EXCEPTION_MESSAGE, playerName, existingPlayerId))
        {
            ExistingPlayerId = existingPlayerId;
        }
    }
}
