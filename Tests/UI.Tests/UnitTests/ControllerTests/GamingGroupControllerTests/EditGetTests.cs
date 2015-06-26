using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    public class EditGetTests : GamingGroupControllerTestBase
    {
        private const int GAMING_GROUP_ID = 1;

        [Test]
        public void ItReturnsGamingGroupEditView()
        {
            //--Arrange
            autoMocker.PartialMockTheClassUnderTest();
            autoMocker.ClassUnderTest.Expect(x => x.Edit(Arg<int>.Is.Anything)).Return(new ViewResult
            {
                ViewName = MVC.GamingGroup.Views.Edit
            });

            //--Act
            var viewResult = autoMocker.ClassUnderTest.Edit(GAMING_GROUP_ID) as ViewResult;

            //--Assert
            Assert.AreEqual(MVC.GamingGroup.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItSendsCorrectModelToView()
        {
            //--Arrange
            var model = new GamingGroupPublicDetailsViewModel
            {
                PublicDescription = "Description",
                Website = "Website"
            };

            autoMocker.PartialMockTheClassUnderTest();
            autoMocker.ClassUnderTest.Expect(x => x.Edit(Arg<int>.Is.Anything)).Return(new ViewResult
            {
                ViewData = new ViewDataDictionary(model)
            });

            //--Act
            var viewResult = autoMocker.ClassUnderTest.Edit(GAMING_GROUP_ID) as ViewResult;

            //--Assert
            Assert.AreEqual(model, viewResult.Model);
        }
    }
}