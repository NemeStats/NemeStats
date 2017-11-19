using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetGamingGroupWithUsersTests : GamingGroupRetrieverTestBase
    {
        private int _gamingGroupId;
        private GamingGroup _expectedGamingGroup;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _gamingGroupId = CurrentUser.CurrentGamingGroupId.Value;

            _expectedGamingGroup = new GamingGroup
            {
                Id = _gamingGroupId,
                Active = false,
                Name = "some gaming group name",
                PublicDescription = "some public description",
                PublicGamingGroupWebsite = "some website"
            };
            AutoMocker.Get<ISecuredEntityValidator>()
                .Expect(mock => mock.RetrieveAndValidateAccess<GamingGroup>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedGamingGroup);
        }

        [Test]
        public void It_Returns_The_Specified_Gaming_Group_Info_If_The_User_Has_Access_To_It()
        {
            //--arrange
            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(new List<Player>().AsQueryable());

            //--act
            var result = AutoMocker.ClassUnderTest.GetGamingGroupWithUsers(_gamingGroupId, CurrentUser);

            //--assert
            AutoMocker.Get<ISecuredEntityValidator>()
                .AssertWasCalled(mock => mock.RetrieveAndValidateAccess<GamingGroup>(Arg<int>.Is.Equal(_gamingGroupId), Arg<ApplicationUser>.Is.Same(CurrentUser)));

            result.Active.ShouldBe(_expectedGamingGroup.Active);
            result.GamingGroupId.ShouldBe(_expectedGamingGroup.Id);
            result.GamingGroupName.ShouldBe(_expectedGamingGroup.Name);
            result.PublicDescription.ShouldBe(_expectedGamingGroup.PublicDescription);
            result.PublicGamingGroupWebsite.ShouldBe(_expectedGamingGroup.PublicGamingGroupWebsite);
        }

        [Test]
        public void It_Returns_The_Users_Who_Are_Associated_With_This_Gaming_Group_Ordered_By_Active_Then_Username()
        {
            //--arrange
            var expectedPlayer1 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Name = "player 1 name",
                ApplicationUserId = "user 1 id",
                Active = true,
                User = new ApplicationUser
                {
                    UserName = "bbb - username 1",
                    Email = "email 1"
                }
            };
            var expectedPlayer2 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Name = "player 2 name",
                ApplicationUserId = "user 2 id",
                Active = true,
                User = new ApplicationUser
                {
                    UserName = "ccc - username 2",
                    Email = "email 2"
                }
            };

            var expectedPlayer3 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Name = "player 3 name",
                ApplicationUserId = "user 3 id",
                Active = false,
                User = new ApplicationUser
                {
                    //--this player will be last because it is inactive, even though its username would sort first
                    UserName = "aaa - username 3",
                    Email = "email 3"
                }
            };

            var players = new List<Player>
            {
                //--this player should be ignored because the gaming group doesn't match
                new Player
                {
                    GamingGroupId = -1,
                    ApplicationUserId = "ignored user id",
                },
                //--this player should be ignored because the applicationUserId is null
                new Player
                {
                    GamingGroupId = _gamingGroupId,
                    ApplicationUserId = null,
                },
                expectedPlayer3,
                expectedPlayer2,
                expectedPlayer1
            }.AsQueryable();
            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(players);

            //--act
            var result = AutoMocker.ClassUnderTest.GetGamingGroupWithUsers(_gamingGroupId, CurrentUser);

            //--assert
            result.OtherUsers.Count.ShouldBe(3);
            var user1 = result.OtherUsers[0];
            user1.Email.ShouldBe(expectedPlayer1.User.Email);
            user1.PlayerName.ShouldBe(expectedPlayer1.Name);
            user1.UserName.ShouldBe(expectedPlayer1.User.UserName);

            var user2 = result.OtherUsers[1];
            user2.Email.ShouldBe(expectedPlayer2.User.Email);
            user2.PlayerName.ShouldBe(expectedPlayer2.Name);
            user2.UserName.ShouldBe(expectedPlayer2.User.UserName);

            var user3 = result.OtherUsers[2];
            user3.Email.ShouldBe(expectedPlayer3.User.Email);
            user3.PlayerName.ShouldBe(expectedPlayer3.Name);
            user3.UserName.ShouldBe(expectedPlayer3.User.UserName);
        }
    }
}
