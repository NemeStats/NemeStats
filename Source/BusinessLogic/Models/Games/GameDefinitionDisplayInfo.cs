using System;
using System.Data.Entity.Migrations.Model;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace BusinessLogic.Models.Games
{
    public class GameDefinitionDisplayInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public int PlayedTimes { get; set; }
        public DateTime? LastDatePlayed { get; set; }
    }
}
