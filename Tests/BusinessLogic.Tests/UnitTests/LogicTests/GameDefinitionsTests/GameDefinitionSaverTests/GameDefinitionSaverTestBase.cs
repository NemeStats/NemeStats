using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionSaverTests
{
    public abstract class GameDefinitionSaverTestBase
    {
        protected RhinoAutoMocker<GameDefinitionSaver> autoMocker;
        protected ApplicationUser currentUser;

        [SetUp]
        public void BaseSetUp()
        {
            autoMocker = new RhinoAutoMocker<GameDefinitionSaver>();
            autoMocker.PartialMockTheClassUnderTest();

            currentUser = new ApplicationUser
            {
                Id = "some application user id",
                CurrentGamingGroupId = 100
            };
        }
    }
}
