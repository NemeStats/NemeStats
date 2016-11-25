namespace BusinessLogic.Models.Players
{
    public class ChampionDataModel
    {
        public int WinPercentage { get; set; }
        public int NumberOfWins { get; set; }
        public int NumberOfGames { get; set; }
        public int GameDefinitionId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int PlayerGamingGroupId { get; set; }
        public string PlayerGamingGroupName { get; set; }
    }
}