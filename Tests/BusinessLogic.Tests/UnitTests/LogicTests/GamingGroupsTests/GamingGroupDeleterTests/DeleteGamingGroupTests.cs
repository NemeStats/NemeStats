using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupDeleterTests
{
    [TestFixture]
    public class DeleteGamingGroupTests
    {
        private RhinoAutoMocker<GamingGroupDeleter> _autoMocker;
        private IDataContext _dataContextMock;
        private ApplicationUser _currentUser;
        private int _gamingGroupId = 1;
        private GamingGroup _expectedGamingGroup;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<GamingGroupDeleter>();
            _autoMocker.PartialMockTheClassUnderTest();

            _dataContextMock = MockRepository.GenerateMock<IDataContext>();
            _currentUser = new ApplicationUser();
            _expectedGamingGroup = new GamingGroup
            {
                GamingGroupId = _gamingGroupId
            };

            _autoMocker.Get<ISecuredEntityValidator>().Expect(mock =>
                    mock.RetrieveAndValidateAccess<GamingGroup>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedGamingGroup);

            _autoMocker.ClassUnderTest.Expect(mock => mock.DeletePlayerGameResults(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            _autoMocker.ClassUnderTest.Expect(mock => mock.DeletePlayedGames(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            _autoMocker.ClassUnderTest.Expect(mock => mock.DeletePlayerAchievements(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            _autoMocker.ClassUnderTest.Expect(mock => mock.DeleteChampionsAndGameDefinitions(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            _autoMocker.ClassUnderTest.Expect(mock => mock.DeleteNemeses(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            _autoMocker.ClassUnderTest.Expect(mock => mock.DeletePlayers(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            _autoMocker.ClassUnderTest.Expect(mock => mock.UnassociateUsers(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            _autoMocker.ClassUnderTest.Expect(mock => mock.DeleteGamingGroupInvitations(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            _autoMocker.ClassUnderTest.Expect(mock => mock.DeleteGamingGroup(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Sets_The_Timeout_To_5_Minutes_Since_It_Could_Take_A_Long_Time_To_Run()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.Execute(_gamingGroupId, _currentUser);

            //--assert
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.SetCommandTimeout(300));
        }

        [Test]
        public void It_Validates_The_User_Has_Ownership_Of_The_Gaming_Group()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.Execute(_gamingGroupId, _currentUser, _dataContextMock);

            //--assert
            _autoMocker.Get<ISecuredEntityValidator>().AssertWasCalled(
                mock => mock.RetrieveAndValidateAccess<GamingGroup>(Arg<int>.Is.Equal(_gamingGroupId), Arg<ApplicationUser>.Is.Same(_currentUser)));
        }

        [Test]
        public void It_Deletes_All_The_Things()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.Execute(_gamingGroupId, _currentUser, _dataContextMock);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeletePlayerGameResults(_gamingGroupId, _currentUser));
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeletePlayedGames(_gamingGroupId, _currentUser));
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeletePlayerAchievements(_gamingGroupId, _currentUser));
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeleteChampionsAndGameDefinitions(_gamingGroupId, _currentUser));
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeleteNemeses(_gamingGroupId, _currentUser));
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeletePlayers(_expectedGamingGroup, _currentUser));
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.UnassociateUsers(_gamingGroupId, _currentUser));
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeleteGamingGroupInvitations(_gamingGroupId, _currentUser));
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeleteGamingGroup(_gamingGroupId, _currentUser));

        }
    }
}
