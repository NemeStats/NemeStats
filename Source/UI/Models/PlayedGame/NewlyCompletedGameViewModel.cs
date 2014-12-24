using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Models.Games;

namespace UI.Models.PlayedGame
{
    public class NewlyCompletedGameViewModel : NewlyCompletedGame
    {
        public IEnumerable<SelectListItem> GameDefinitions { get; set; }
        public IEnumerable<SelectListItem> Players { get; set; }

    }
}