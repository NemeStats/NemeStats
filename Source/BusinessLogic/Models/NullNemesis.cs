using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    [NotMapped]
    public class NullNemesis : Nemesis
    {
        public NullNemesis()
        {
            NemesisPlayer = new Player();
            MinionPlayer = new Player();
        }
    }
}
