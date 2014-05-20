using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class Player
    {
        public int Id { get; set; }

        public string Name { get; set; }
        [DefaultValue("true")]
        public bool Active { get; set; }
    }
}
