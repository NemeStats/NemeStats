using System.Linq;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.User;
using NUnit.Framework;
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
