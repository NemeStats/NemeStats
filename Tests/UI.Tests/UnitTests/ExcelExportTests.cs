using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BusinessLogic.Models;
using CsvHelper;
using NUnit.Framework;

namespace UI.Tests.UnitTests
{
    [TestFixture]
    public class ExcelExportTests
    {
        [Test, Ignore]
        public void ItCreatesAnExcelFile()
        { 
            List<PlayedGame> playedGames = new List<PlayedGame>();
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    HttpResponse response = new HttpResponse(streamWriter);
                    var csvWriter = new CsvWriter(streamWriter);

                    csvWriter.WriteRecords(playedGames);
                }
                
            }
            
        }
    }
}
