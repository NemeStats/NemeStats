using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Shouldly;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class EditGetTests : GamingGroupControllerTestBase
    {
        private const int GAMING_GROUP_ID = 1;

        [Test]
        public void It_Returns_The_Specified_Gaming_Group_As_The_Model_For_The_Edit_View()
        {
            //--Arrange
            var gamingGroup = new GamingGroup
            {
                Active = !default(bool),
                Name = "some gaming group name",
                PublicDescription = "some public description",
                PublicGamingGroupWebsite = "some website url"
            };
            autoMocker.Get<ISecuredEntityValidator>().Expect(mock => mock.RetrieveAndValidateAccess<GamingGroup>(GAMING_GROUP_ID, currentUser))
                .Return(gamingGroup);

            //--Act
            var viewResult = autoMocker.ClassUnderTest.Edit(GAMING_GROUP_ID, currentUser) as ViewResult;

            //--Assert
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.GamingGroup.Views.Edit);
            var viewModel = viewResult.Model as GamingGroupPublicDetailsViewModel;
            viewModel.ShouldNotBeNull();
            viewModel.Active.ShouldBe(gamingGroup.Active);
            viewModel.GamingGroupName.ShouldBe(gamingGroup.Name);
            viewModel.PublicDescription.ShouldBe(gamingGroup.PublicDescription);
            viewModel.Website.ShouldBe(gamingGroup.PublicGamingGroupWebsite);

        }
    }
}