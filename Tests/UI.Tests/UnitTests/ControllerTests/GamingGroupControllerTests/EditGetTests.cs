using NUnit.Framework;
using System.Web.Mvc;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class EditGetTests : GamingGroupControllerTestBase
    {
        private const int GAMING_GROUP_ID = 1;

        [Test]
        public void ItReturnsTheGamingGroupEditViewModel()
        {
            var viewResult = autoMocker.ClassUnderTest.Edit(GAMING_GROUP_ID) as ViewResult;

            Assert.AreEqual(MVC.GamingGroup.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItSendsCorrectModelToView()
        {
            //--Arrange
            var model = new GamingGroupPublicDetailsModel
            {
                PublicDescription = "Awesome Gaming Group",
            };

            //--Act
            var viewResult = autoMocker.ClassUnderTest.Edit(GAMING_GROUP_ID) as ViewResult;

            //--Assert
            Assert.AreEqual(model, viewResult.Model);
        }

        //[Test]
        //public void ItReturnsSpecifiedTopGamingGroupsModelToTheView()
        //{
        //    var viewResult = gamingGroupControllerPartialMock.GetTopGamingGroups() as ViewResult;

        // var actualViewModel = viewResult.ViewData.Model;

        //    Assert.AreEqual(expectedViewModel, actualViewModel);
        //}
    }
}