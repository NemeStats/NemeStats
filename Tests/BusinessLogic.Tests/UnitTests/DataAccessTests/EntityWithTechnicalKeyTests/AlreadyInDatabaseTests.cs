using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
