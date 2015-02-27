using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.PlayedGames;
using OfficeOpenXml;

namespace BusinessLogic.Export
{
    public class ExcelGenerator : IExcelGenerator
    {
        private const int START_ROW_OF_NON_HEADER_DATA = 2;

        public void GenerateExcelFile(List<PlayedGameExportModel> playedGameExportModels, MemoryStream exportMemoryStream)
        {
            using (var package = new ExcelPackage(exportMemoryStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("NemeStats Played Games");
                worksheet.Cells[1, 1].Value = "Game Name";

                for (int i = 0; i < playedGameExportModels.Count(); i++)
                {
                    PlayedGameExportModel playedGameExportModel = playedGameExportModels[i];

                    worksheet.Cells[i + START_ROW_OF_NON_HEADER_DATA, 1].Value = playedGameExportModel.GameDefinitionName;
                }

                package.SaveAs(exportMemoryStream);
            }

            exportMemoryStream.Position = 0;
        }
    }
}
