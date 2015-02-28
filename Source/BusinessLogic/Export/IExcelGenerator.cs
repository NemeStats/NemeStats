using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Export
{
    public interface IExcelGenerator
    {
        void GenerateExcelFile(List<PlayedGameExportModel> playedGameExportModel, MemoryStream memoryStream);
    }
}
