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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BusinessLogic.Models.PlayedGames;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BusinessLogic.Export
{
    public class ExcelGenerator : IExcelGenerator
    {
        private const int START_ROW_OF_NON_HEADER_DATA = 2;

        private const int INDEX_OF_PLAYED_GAME_ID = 1;
        private const int INDEX_OF_DATE_PLAYED = 2;
        private const int INDEX_OF_GAME_DEFINITION_ID = 3;
        private const int INDEX_OF_GAME_DEFINITION_NAME = 4;
        private const int INDEX_OF_BOARD_GAME_GEEK_OBJECT_ID = 5;
        private const int INDEX_OF_GAMING_GROUP_ID = 6;
        private const int INDEX_OF_GAMING_GROUP_NAME = 7;
        private const int INDEX_OF_DATE_RECORDED = 8;
        private const int INDEX_OF_NOTES = 9;
        private const int INDEX_OF_NUMBER_OF_PLAYERS = 10;
        private const int INDEX_OF_WINNING_PLAYER_IDS = 11;
        private const int INDEX_OF_WINNING_PLAYER_NAMES = 12;

        public void GenerateExcelFile(List<PlayedGameExportModel> playedGameExportModels, MemoryStream exportMemoryStream)
        {
            using (var package = new ExcelPackage(exportMemoryStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("NemeStats Played Games");
                AddHeaderRow(worksheet);

                worksheet.Column(INDEX_OF_DATE_PLAYED).Style.Numberformat.Format = "yyyy-mm-dd";
                worksheet.Column(INDEX_OF_DATE_RECORDED).Style.Numberformat.Format = "yyyy-mm-dd";

                AddDataRows(playedGameExportModels, worksheet);

                package.SaveAs(exportMemoryStream);
            }

            exportMemoryStream.Position = 0;
        }

        private static void AddDataRows(List<PlayedGameExportModel> playedGameExportModels, ExcelWorksheet worksheet)
        {
            for (int i = 0; i < playedGameExportModels.Count; i++)
            {
                PlayedGameExportModel playedGameExportModel = playedGameExportModels[i];

                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_PLAYED_GAME_ID].Value = playedGameExportModel.Id;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_DATE_PLAYED].Value = playedGameExportModel.DatePlayed;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_GAME_DEFINITION_ID].Value = playedGameExportModel.GameDefinitionId;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_GAME_DEFINITION_NAME].Value = playedGameExportModel.GameDefinitionName;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_BOARD_GAME_GEEK_OBJECT_ID].Value = playedGameExportModel.BoardGameGeekGameDefinitionId;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_GAMING_GROUP_ID].Value = playedGameExportModel.GamingGroupId;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_GAMING_GROUP_NAME].Value = playedGameExportModel.GamingGroupName;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_DATE_RECORDED].Value = playedGameExportModel.DateCreated;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_NOTES].Value = playedGameExportModel.Notes;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_NUMBER_OF_PLAYERS].Value = playedGameExportModel.NumberOfPlayers;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_WINNING_PLAYER_IDS].Value = playedGameExportModel.WinningPlayerIds;
                worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, INDEX_OF_WINNING_PLAYER_NAMES].Value = playedGameExportModel.WinningPlayerNames;
            }
        }

        private static void AddHeaderRow(ExcelWorksheet worksheet)
        {
            worksheet.Cells[1, 1].Value = "Game Name";
            worksheet.Cells[1, INDEX_OF_PLAYED_GAME_ID].Value = "Played Game Id";
            worksheet.Cells[1, INDEX_OF_DATE_PLAYED].Value = "Date Played (UTC)";
            worksheet.Cells[1, INDEX_OF_GAME_DEFINITION_ID].Value = "Game Id";
            worksheet.Cells[1, INDEX_OF_GAME_DEFINITION_NAME].Value = "Game Name";
            worksheet.Cells[1, INDEX_OF_BOARD_GAME_GEEK_OBJECT_ID].Value = "BoardGameGeek Object Id";
            worksheet.Cells[1, INDEX_OF_GAMING_GROUP_ID].Value = "Gaming Group Id";
            worksheet.Cells[1, INDEX_OF_GAMING_GROUP_NAME].Value = "Gaming Group Name";
            worksheet.Cells[1, INDEX_OF_DATE_RECORDED].Value = "Date Recorded (UTC)";
            worksheet.Cells[1, INDEX_OF_NOTES].Value = "Notes";
            worksheet.Cells[1, INDEX_OF_NUMBER_OF_PLAYERS].Value = "Number Of Players";
            worksheet.Cells[1, INDEX_OF_WINNING_PLAYER_IDS].Value = "Winning Player Ids";
            worksheet.Cells[1, INDEX_OF_WINNING_PLAYER_NAMES].Value = "Winning Player Names";

            using (var range = worksheet.Cells[1, 1, 1, 12])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Black);
                range.Style.Font.Color.SetColor(Color.WhiteSmoke);
                range.Style.ShrinkToFit = false;
                range.AutoFitColumns();
            }
        }
    }
}
