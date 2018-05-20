using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GetGamingGroupPlayersTests : GamingGroupControllerTestBase
    {
        [Test]
        public void It_Returns_The_Players_In_The_Specific_Gaming_Group_With_The_Specified_Date_Filter()
        {
            //--arrange
            var gamingGroupId = 1;
            var currentUser = new ApplicationUser();
            var dateRangeFilter = new BasicDateRangeFilter();
            var expectedResults = new List<PlayerWithNemesis>
            {
                new PlayerWithNemesis(),
                new PlayerWithNemesis()
            };
            var expectedResult1 = new PlayerWithNemesisViewModel();
            var expectedResult2 = new PlayerWithNemesisViewModel();

            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetAllPlayersWithNemesisInfo(gamingGroupId, dateRangeFilter)).Return(expectedResults);
            autoMocker.Get<IPlayerWithNemesisViewModelBuilder>().Expect(mock => mock.Build(expectedResults[0], currentUser)).Return(expectedResult1);
            autoMocker.Get<IPlayerWithNemesisViewModelBuilder>().Expect(mock => mock.Build(expectedResults[1], currentUser)).Return(expectedResult2);

            //--act
            var result = autoMocker.ClassUnderTest.GetGamingGroupPlayers(gamingGroupId, currentUser, dateRangeFilter);

            //--assert
            var viewResult = result as PartialViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.Player.Views._PlayersPartial);
            var model = viewResult.Model as List<PlayerWithNemesisViewModel>;
            model.ShouldNotBeNull();
            model.Count.ShouldBe(2);
            model[0].ShouldBeSameAs(expectedResult1);
            model[1].ShouldBeSameAs(expectedResult2);
        }

        [Test]
        public void It_Indicates_Whether_There_Are_Any_Players_With_Gravatars()
        {
            //--arrange

            //--act

            //--assert

        }
    }
}
