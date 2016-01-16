using System;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;

namespace UI.Models.User
{
    public class BoardGameGeekIntegrationModel
    {
        [Required]
        public string BoardGameGeekUserName { get; set; }

        public string AvatarUrl { get; set; }

        public Uri BoardGameGeekUserUrl { get; set; }

        public bool IntegrationComplete { get; set; }
    }
}