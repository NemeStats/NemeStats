using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using Shouldly;
using UI.Models.GamingGroup;
using UI.Models.User;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class EditGetTests : GamingGroupControllerTestBase
    {
        private const int GAMING_GROUP_ID = 1;

        [Test]
        public void It_Returns_The_Specified_Gaming_Group_As_The_Model_For_The_Edit_View()
        {
            //--Arrange
            var gamingGroup = new GamingGroupWithUsers
            {
                GamingGroupId = GAMING_GROUP_ID,
                Active = !default(bool),
                GamingGroupName = "some gaming group name",
                PublicDescription = "some public description",
                PublicGamingGroupWebsite = "some website url",
                OtherUsers = new List<BasicUserInfo>
                {
                    new BasicUserInfo
                    {
                        UserName = "username 1"
                    },
                    new BasicUserInfo
                    {
                        UserName = "username 2"
                    }
                },
                UserCanDelete = true
            };
            autoMocker.Get<IGamingGroupRetriever>().Expect(mock => mock.GetGamingGroupWithUsers(GAMING_GROUP_ID, currentUser))
                .Return(gamingGroup);
            var expectedBasicUserInfoViewModel1 = new BasicUserInfoViewModel();
            autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<BasicUserInfoViewModel>(gamingGroup.OtherUsers[0]))
                .Return(expectedBasicUserInfoViewModel1);
            var expectedBasicUserInfoViewModel2 = new BasicUserInfoViewModel();
            autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<BasicUserInfoViewModel>(gamingGroup.OtherUsers[1]))
                .Return(expectedBasicUserInfoViewModel2);

            //--Act
            var viewResult = autoMocker.ClassUnderTest.Edit(GAMING_GROUP_ID, currentUser) as ViewResult;

            //--Assert
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.GamingGroup.Views.Edit);
            var viewModel = viewResult.Model as GamingGroupPublicDetailsViewModel;
            viewModel.ShouldNotBeNull();
            viewModel.GamingGroupId.ShouldBe(gamingGroup.GamingGroupId);
            viewModel.Active.ShouldBe(gamingGroup.Active);
            viewModel.GamingGroupName.ShouldBe(gamingGroup.GamingGroupName);
            viewModel.PublicDescription.ShouldBe(gamingGroup.PublicDescription);
            viewModel.Website.ShouldBe(gamingGroup.PublicGamingGroupWebsite);
            viewModel.OtherUsers.Count.ShouldBe(2);
            viewModel.OtherUsers[0].ShouldBeSameAs(expectedBasicUserInfoViewModel1);
            viewModel.OtherUsers[1].ShouldBeSameAs(expectedBasicUserInfoViewModel2);
            viewModel.UserCanDelete.ShouldBe(gamingGroup.UserCanDelete);
        }
    }
}