using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.User;
using NUnit.Framework;
using StructureMap.AutoMocking;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.UniversalGameControllerTests
{
    [TestFixture]
    public class DetailsTests
    {
        private RhinoAutoMocker<UniversalGameController> _autoMocker;

        private int _boardGameGeekGameDefinitionId = 1;
        private ApplicationUser _currentUser;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalGameController>();
            _currentUser = new ApplicationUser();
        }

        [Test]
        public void It_Returns_A_UniversalGameViewModel()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser);

            //--assert
        }

    }
}
