using BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class BoardGameGeekGameDefinition : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
    }
}
