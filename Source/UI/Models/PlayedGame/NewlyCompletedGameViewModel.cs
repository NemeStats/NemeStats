using BusinessLogic.Models.Games;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UI.Models.PlayedGame
{
	public class NewlyCompletedGameViewModel : NewlyCompletedGame
	{
		public IEnumerable<SelectListItem> GameDefinitions { get; set; }

		public IEnumerable<SelectListItem> Players { get; set; }

		public Dictionary<string, int> ExistingRankedPlayerNames { get; set; }

		public int PreviousGameId { get; set; }

		public NewlyCompletedGameViewModel()
		{
			this.PreviousGameId = -1;
		}
	}
}