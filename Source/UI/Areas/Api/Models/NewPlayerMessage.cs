using System.ComponentModel.DataAnnotations;

namespace UI.Areas.Api.Models
{
    public class NewPlayerMessage
    {
        [Required]
        public string PlayerName { get; set; }
    }
}