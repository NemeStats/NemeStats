using System.ComponentModel.DataAnnotations;
using System.Linq;

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
