using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.GameDefinitionModels;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetGamingGroupGameDefinitionsTests : GamingGroupControllerTestBase
    {
        [Test]
        public void It_Returns_The_Game_Definitions_In_The_Specific_Gaming_Group_With_The_Specified_Date_Filter()
        {
            //--arrange
            var gamingGroupId = 1;
            var currentUser = new ApplicationUser();
            var dateRangeFilter = new BasicDateRangeFilter();
            var expectedResults = new List<GameDefinitionSummary>
            {
                new GameDefinitionSummary(),
                new GameDefinitionSummary()
            };
            var expectedResult1 = new GameDefinitionSummaryViewModel();
            var expectedResult2 = new GameDefinitionSummaryViewModel();

            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitions(gamingGroupId, dateRangeFilter)).Return(expectedResults);
            autoMocker.Get<IGameDefinitionSummaryViewModelBuilder>().Expect(mock => mock.Build(expectedResults[0], currentUser)).Return(expectedResult1);
            autoMocker.Get<IGameDefinitionSummaryViewModelBuilder>().Expect(mock => mock.Build(expectedResults[1], currentUser)).Return(expectedResult2);

            //--act
            var result = autoMocker.ClassUnderTest.GetGamingGroupGameDefinitions(gamingGroupId, currentUser, dateRangeFilter);

            //--assert
            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.GameDefinition.Views._GameDefinitionsPartial);
            var model = viewResult.Model as List<GameDefinitionSummaryViewModel>;
            model.ShouldNotBeNull();
            model.Count.ShouldBe(2);
            model[0].ShouldBeSameAs(expectedResult1);
            model[1].ShouldBeSameAs(expectedResult2);
        }

    }
}
