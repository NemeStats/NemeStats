using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetGamingGroupPlayedGamesTests : GamingGroupControllerTestBase
    {
        [Test]
        public void It_Returns_The_Specified_Number_Of_Played_Games_In_The_Specific_Gaming_Group_With_The_Specified_Date_Filter()
        {
            //--arrange
            var gamingGroupId = 1;
            var currentUser = new ApplicationUser();
            var dateRangeFilter = new BasicDateRangeFilter();
            var numberOfItems = 2;

            var expectedPlayedGames = new List<PlayedGame>
            {
                new PlayedGame(),
                new PlayedGame()
            };
            var expectedResult1 = new PlayedGameDetailsViewModel();
            var expectedResult2 = new PlayedGameDetailsViewModel();

            autoMocker.Get<IPlayedGameRetriever>()
                .Expect(mock => mock.GetRecentGames(numberOfItems, gamingGroupId, dateRangeFilter))
                .Return(expectedPlayedGames);
            autoMocker.Get<IPlayedGameDetailsViewModelBuilder>()
                .Expect(mock => mock.Build(expectedPlayedGames[0], currentUser))
                .Return(expectedResult1);
            autoMocker.Get<IPlayedGameDetailsViewModelBuilder>()
                .Expect(mock => mock.Build(expectedPlayedGames[1], currentUser))
                .Return(expectedResult2);

            //--act
            var result = autoMocker.ClassUnderTest.GetGamingGroupPlayedGames(gamingGroupId, currentUser, dateRangeFilter, numberOfItems);

            //--assert
            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.PlayedGame.Views._PlayedGamesPartial);
            var model = viewResult.Model as PlayedGamesViewModel;
            model.ShouldNotBeNull();
            model.GamingGroupId.ShouldBe(gamingGroupId);
            model.ShowSearchLinkInResultsHeader.ShouldBeTrue();
            model.UserCanEdit.ShouldBeFalse("UserCanEdit should only be true if the Gaming Group is the current user's Gaming Group.");
            model.PlayedGameDetailsViewModels.ShouldNotBeNull();
            model.PlayedGameDetailsViewModels.Count.ShouldBe(2);
            model.PlayedGameDetailsViewModels[0].ShouldBeSameAs(expectedResult1);
            model.PlayedGameDetailsViewModels[1].ShouldBeSameAs(expectedResult2);
        }

        public void Users_Can_Edit_If_This_Is_Their_Gaming_Group()
        {
            //--arrange
            var gamingGroupId = 1;
            var currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = gamingGroupId
            };
            var dateRangeFilter = new BasicDateRangeFilter();

            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(0, 0, null)).IgnoreArguments().Return(new List<PlayedGame>());

            //--act
            var result = autoMocker.ClassUnderTest.GetGamingGroupPlayedGames(gamingGroupId, currentUser, dateRangeFilter, 0);

            //--assert
            var viewResult = result as ViewResult;
            viewResult.ShouldNotBeNull();
            var model = viewResult.Model as PlayedGamesViewModel;
            model.ShouldNotBeNull();
            model.UserCanEdit.ShouldBeTrue();
        }
    }
}
