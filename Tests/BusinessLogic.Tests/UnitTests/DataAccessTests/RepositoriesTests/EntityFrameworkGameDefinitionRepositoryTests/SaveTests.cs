using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests
{
    [TestFixture]
    public class SaveTests : EntityFrameworkGameDefinitionRepositoryTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            gameDefinitionsDbSetMock.Expect(mock => mock.Add(gameDefinition));
        }
    }
}
