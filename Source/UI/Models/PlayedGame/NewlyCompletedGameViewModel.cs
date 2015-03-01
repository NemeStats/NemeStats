using BusinessLogic.Models.Games;
using BusinessLogic.Models.Games.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UI.Models.PlayedGame
{
	public class NewlyCompletedGameViewModel : NewlyCompletedGame
	{
		public IEnumerable<SelectListItem> GameDefinitions { get; set; }

		public IEnumerable<SelectListItem> Players { get; set; }

		public Dictionary<string, int> ExistingRankedPlayerNames { get; set; }

		[PlayerRankValidationAttribute]
		[Required]
		public List<PlayerRank> PlayerRanks { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime DatePlayed { get; set; }

		public int PreviousGameId { get; set; }

		//public NewlyCompletedGameViewModel()
		//{
		//	this.PreviousGameId = -1;
		//}
	}
}