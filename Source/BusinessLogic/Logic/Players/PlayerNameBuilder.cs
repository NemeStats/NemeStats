namespace BusinessLogic.Logic.Players
{
    public class PlayerNameBuilder
    {
        public static string BuildPlayerName(string playerName, bool active)
        {
            if (active)
            {
                return playerName;
            }

            return $"{playerName} (INACTIVE)";
        }
    }
}
