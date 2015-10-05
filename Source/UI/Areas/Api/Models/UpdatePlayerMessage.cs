using System.Linq;

namespace UI.Areas.Api.Models
{
    public class UpdatePlayerMessage
    {
        public string PlayerName { get; set; }
        public bool? Active { get; set; }
    }
}
    