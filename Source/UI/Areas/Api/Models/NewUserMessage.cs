using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class NewUserMessage
    {
        public string EmailAddress { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
