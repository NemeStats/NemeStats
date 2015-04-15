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
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Exceptions
{
    public class EntityDoesNotExistException : KeyNotFoundException
    {
        internal const string EXCEPTION_MESSAGE_FORMAT = "Entity of type '{0}' with Id '{1}' does not exist.";

        private readonly object entityId;
        private readonly Type entityType;

        public EntityDoesNotExistException(Type entityType, object entityId)
        {
            this.entityId = entityId;
            this.entityType = entityType;
        }

        public override string Message
        {
            get
            {
                return string.Format(EXCEPTION_MESSAGE_FORMAT, entityType.Name, entityId);
            }
        }
    }
}
