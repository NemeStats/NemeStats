using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.AchievementControllerTests
{
    [TestFixture]
    public class PlayerAchievementTests
    {
        private RhinoAutoMocker<AchievementController> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<AchievementController>();
        }

        [Test]
        public void It_Returns_A_404_If_The_Player_Doesnt_Have_The_Achievement()
        {
            //--arrange
            var achievementId = AchievementId.BusyBee;
            _autoMocker.Get<IPlayerAchievementRetriever>().Expect(mock => mock.GetPlayerAchievement(0, achievementId))
                .IgnoreArguments()
                .Return(null);

            //--act
            var result = _autoMocker.ClassUnderTest.PlayerAchievement(achievementId, 0);

            //--assert
            result.ShouldBeAssignableTo<HttpNotFoundResult>();
        }

    }
}
