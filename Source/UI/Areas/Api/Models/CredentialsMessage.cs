using System.ComponentModel.DataAnnotations;

namespace UI.Areas.Api.Models
{
    public class CredentialsMessage
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }

        public bool PreserveExistingAuthenticationToken { get; set; }
    }
}
