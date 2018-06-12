using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Utility;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
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
            var dateRangeFilter = new BasicDateRangeFilter();
            var expectedResults = new List<PlayerWithNemesis>
            {
                new PlayerWithNemesis(),
                new PlayerWithNemesis()
            };

            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetAllPlayersWithNemesisInfo(gamingGroupId, dateRangeFilter)).Return(expectedResults);
            autoMocker.Get<IPlayerRetriever>().Expect(mock =>
                    mock.GetRegisteredUserEmailAddresses(
                        Arg<List<int>>.Is.Anything, 
                        Arg<ApplicationUser>.Is.Anything))
                .Return(new Dictionary<int, string>());

            var expectedResult1 = new PlayerWithNemesisViewModel();
            var expectedResult2 = new PlayerWithNemesisViewModel();
            autoMocker.Get<IPlayerWithNemesisViewModelBuilder>().Expect(mock => mock.Build(expectedResults[0], null, currentUser)).Return(expectedResult1);
            autoMocker.Get<IPlayerWithNemesisViewModelBuilder>().Expect(mock => mock.Build(expectedResults[1], null, currentUser)).Return(expectedResult2);

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

            var gamingGroupId = 1;
            var expectedPlayerIdWithoutEmail = 1;
            var expectedPlayerIdWithEmail = 2;
            var expectedResults = new List<PlayerWithNemesis>
            {
                new PlayerWithNemesis
                {
                    PlayerId = expectedPlayerIdWithEmail
                },
                new PlayerWithNemesis
                {
                    PlayerId = expectedPlayerIdWithoutEmail
                }
            };

            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetAllPlayersWithNemesisInfo(gamingGroupId, null)).Return(expectedResults);

            var expectedEmail = "email@email.com";
            var playerToRegisteredEmailDictionary = new Dictionary<int, string>
            {
                { 2, expectedEmail }
            };
            autoMocker.Get<IPlayerRetriever>().Expect(mock =>
                    mock.GetRegisteredUserEmailAddresses(
                        Arg<List<int>>.Is.Anything,
                        Arg<ApplicationUser>.Is.Anything))
                .Return(playerToRegisteredEmailDictionary);

            autoMocker.Get<IPlayerWithNemesisViewModelBuilder>().Expect(mock => mock.Build(null, null, null)).IgnoreArguments().Return(new PlayerWithNemesisViewModel());


            //--act
            autoMocker.ClassUnderTest.GetGamingGroupPlayers(gamingGroupId, currentUser);

            //--assert
            var args = autoMocker.Get<IPlayerRetriever>().GetArgumentsForCallsMadeOn(mock =>
                mock.GetRegisteredUserEmailAddresses(
                    Arg<List<int>>.Is.Anything,
                    Arg<ApplicationUser>.Is.Anything));
            var playerIds = args.AssertFirstCallIsType<List<int>>(0);
            playerIds.Count.ShouldBe(2);
            playerIds.ShouldContain(expectedPlayerIdWithEmail);
            playerIds.ShouldContain(expectedPlayerIdWithoutEmail);

            var actualUser = args.AssertFirstCallIsType<ApplicationUser>(1);
            actualUser.ShouldBeSameAs(currentUser);

            var argsForViewModelBuilder = autoMocker.Get<IPlayerWithNemesisViewModelBuilder>()
                .GetArgumentsForCallsMadeOn(mock => mock.Build(null, null, null));
            var actualEmail = argsForViewModelBuilder.AssertFirstCallIsType<string>(1);
            actualEmail.ShouldBe(expectedEmail);
        }
    }
}
