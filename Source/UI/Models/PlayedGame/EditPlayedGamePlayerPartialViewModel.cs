namespace UI.Models.PlayedGame
{
	public class EditPlayedGamePlayerPartialViewModel
	{
		public int? PlayerId { get; set; }

		public string PlayerName { get; set; }

		public int? GameRank { get; set; }

		public EditPlayedGamePlayerPartialViewModel()
		{
		}

		public EditPlayedGamePlayerPartialViewModel(int? playerId, string playerName)
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
			//this.GameRank = gameRank;
		}
	}
}