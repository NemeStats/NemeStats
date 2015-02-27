using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using OfficeOpenXml;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests
{
    [TestFixture]
    public class ExcelExportTests
    {
        [Test, Ignore]
        public void ItCreatesAnExcelFile()
        {
            ExcelWorksheet worksheet;
            var playedGames = new List<PlayedGameExportModel>
            {
                new PlayedGameExportModel
                {
                    DateCreated = DateTime.UtcNow,
                    DatePlayed = DateTime.UtcNow.AddDays(-2),
                    GameDefinitionId = 1,
                    GamingGroupId = 12,
                    Id = 123,
                    Notes = "some notes",
                    NumberOfPlayers = 3,
                    BoardGameGeekObjectId = 1234,
                    GameDefinitionName = "the game name",
                    WinningPlayerIds = "12345|123456",
                    WinningPlayerNames = "Jim|Jane"
                }
            };
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    worksheet = package.Workbook.Worksheets.Add("NemeStats Played Games");

                    worksheet.Cells[1, 1].Value = playedGames[0].GameDefinitionName;
                    FileInfo file = new FileInfo(@"C:\temp\nemestats_export.xlsx");
                    package.SaveAs(file);
                }
            }
        }
    }
}
