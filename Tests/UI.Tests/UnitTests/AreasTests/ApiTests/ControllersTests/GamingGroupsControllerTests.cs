using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Export;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    [TestFixture]
    public class GamingGroupsControllerTests
    {
        private RhinoAutoMocker<GamingGroupsController> autoMocker;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<GamingGroupsController>();
        }

        [Test]
        public void GetPlayedGames_returns_an_excel_file_of_all_played_games_for_a_given_gaming_group()
        {
            int gamingGroupId = 1;
            List<PlayedGame> expectedPlayedGames = new List<PlayedGame>();

            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(GamingGroupsController.MAX_PLAYED_GAMES_TO_EXPORT, gamingGroupId))
                        .Return(expectedPlayedGames);

            HttpResponseMessage httpResponseMessage = autoMocker.ClassUnderTest.GetPlayedGames(gamingGroupId);
            autoMocker.Get<IExcelGenerator>().AssertWasCalled(
                mock => mock.GenerateExcelFile(Arg<List<PlayedGameExportModel>>.Is.Anything, Arg<MemoryStream>.Is.Anything));
        }
    }
}
