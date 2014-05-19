using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class PlayerGameResult
    {
        public int ID { get; set; }
        public int PlayedGameID { get; set; }
        public int PlayerID { get; set; }

        public virtual PlayedGame CompletedGame { get; set; }
        public virtual Player PlayerOfGame { get; set; }
    }
}
