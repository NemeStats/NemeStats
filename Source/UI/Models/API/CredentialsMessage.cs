using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UI.Models.API
{
    public class CredentialsMessage
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
