using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Models.PlayedGame
{
    public class SearchViewModel
    {
        public IEnumerable<SelectListItem> GameDefinitions { get; set; }
        public string DatePlayedStart { get; set; }
        public string DatePlayedEnd { get; set; }
    }
}