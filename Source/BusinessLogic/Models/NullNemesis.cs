using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
