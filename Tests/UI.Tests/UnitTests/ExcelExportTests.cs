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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using OfficeOpenXml;

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
