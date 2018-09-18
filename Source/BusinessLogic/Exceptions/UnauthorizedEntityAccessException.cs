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
using System.Net;

namespace BusinessLogic.Exceptions
{
    public class UnauthorizedEntityAccessException : ApiFriendlyException 
    {
        internal const string EXCEPTION_MESSAGE_FORMAT = "User with Id '{0}' does not have access to entity of type '{1}' with Id '{2}' in Gaming Group '{3}'";

        public UnauthorizedEntityAccessException(string userId, Type entityType, object entityId, int gamingGroupId) 
            : base(string.Format(EXCEPTION_MESSAGE_FORMAT, userId, entityType, entityId, gamingGroupId), HttpStatusCode.Unauthorized)
        {
        }
    }
}
