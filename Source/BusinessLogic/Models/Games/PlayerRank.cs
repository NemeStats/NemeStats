using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Games
{
    public class PlayerRank
    {
        [Required]
        public int? PlayerId { get; set; }
        [Required]
        public int? GameRank { get; set; }
    }
}
