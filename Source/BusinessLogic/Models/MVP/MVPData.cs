using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.MVPData
{
    public class MVPData
    {
        public decimal PointsScored { get; set; }

        public DateTime DatePlayed { get; set; }

        public int Id { get; set; }
        public int PlayerId { get; set; }
    }
}
