using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.BoardGameGeekUserSaverTests
{
    [TestFixture]
    public abstract class BoardGameGeekUserSaverTestBase
    {
        protected RhinoAutoMocker<BoardGameGeekUserSaver> autoMocker;
        protected ApplicationUser currentUser;

        [SetUp]
        public void BaseSetUp()
        {
            autoMocker = new RhinoAutoMocker<BoardGameGeekUserSaver>();
            autoMocker.PartialMockTheClassUnderTest();

            currentUser = new ApplicationUser
            {
                Id = "some application user id",
                CurrentGamingGroupId = 100
            };
        }
    }
}