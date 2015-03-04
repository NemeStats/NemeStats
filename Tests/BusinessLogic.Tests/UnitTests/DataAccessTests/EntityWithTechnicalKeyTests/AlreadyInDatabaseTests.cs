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
using BusinessLogic.DataAccess;
using NUnit.Framework;
using System;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.EntityWithTechnicalKeyTests
{
    [TestFixture]
    public class AlreadyInDatabaseTests
    {
        [Test]
        public void ItReturnsFalseIfTheIdIsNull()
        {
            EntityWithStringKey entityWithStringKey = new EntityWithStringKey();

            Assert.False(entityWithStringKey.AlreadyInDatabase());
        }

        [Test]
        public void ItReturnsFalseIfTheIdIsNotSetForAnIntKey()
        {
            EntityWithIntKey entityWithIntKey = new EntityWithIntKey();

            Assert.False(entityWithIntKey.AlreadyInDatabase());
        }

        [Test]
        public void ItReturnsFalseIfTheIdIsNotSetForAGuidKey()
        {
            EntityWithGuidKey entityWithGuidKey = new EntityWithGuidKey();

            Assert.False(entityWithGuidKey.AlreadyInDatabase());
        }

        private class EntityWithGuidKey : EntityWithTechnicalKey<Guid>
        {
            public override Guid Id { get; set; }
        }

        private class EntityWithStringKey : EntityWithTechnicalKey<string>
        {
            public override string Id { get; set; }
        }

        private class EntityWithIntKey : EntityWithTechnicalKey<int>
        {
            public override int Id { get; set; }
        }
    }
}
