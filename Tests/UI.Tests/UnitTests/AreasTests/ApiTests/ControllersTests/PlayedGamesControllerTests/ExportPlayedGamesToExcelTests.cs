#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.Export;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using UI.Areas.Api.Controllers;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayedGamesControllerTests
{
    [TestFixture]
    public class ExportPlayedGamesToExcelTests
    {
        private RhinoAutoMocker<PlayedGamesController> autoMocker;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<PlayedGamesController>();
        }

        [Test]
        public void GetPlayedGames_returns_an_excel_file_of_all_played_games_for_a_given_gaming_group()
        {
            int gamingGroupId = 1;
            List<PlayedGame> expectedPlayedGames = new List<PlayedGame>
            {
                new PlayedGame
                {
                    GameDefinition = new GameDefinition(),
                    GamingGroup = new GamingGroup(),
                    PlayerGameResults = new List<PlayerGameResult>()
                }
            };
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(PlayedGamesController.MaxPlayedGamesToExport, gamingGroupId))
                        .Return(expectedPlayedGames);

            autoMocker.ClassUnderTest.ExportPlayedGamesToExcel(gamingGroupId);

            autoMocker.Get<IExcelGenerator>().AssertWasCalled(
                mock => mock.GenerateExcelFile(Arg<List<PlayedGameExportModel>>.Is.Anything, Arg<MemoryStream>.Is.Anything));
        }

        [Test]
        public void GetPlayedGames_returns_a_not_found_response_when_there_are_no_played_games()
        {
            int gamingGroupId = 1;
            List<PlayedGame> expectedPlayedGames = new List<PlayedGame>();
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(PlayedGamesController.MaxPlayedGamesToExport, gamingGroupId))
                        .Return(expectedPlayedGames);

            HttpResponseMessage response = autoMocker.ClassUnderTest.ExportPlayedGamesToExcel(gamingGroupId);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
