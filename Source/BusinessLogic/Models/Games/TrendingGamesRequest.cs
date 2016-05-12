namespace BusinessLogic.Models.Games
{
    public class TrendingGamesRequest
    {
        public TrendingGamesRequest(int numberOfTrendingGamesToShow, int numberOfDaysOfTrendingGames)
        {
            NumberOfTrendingGamesToShow = numberOfTrendingGamesToShow;
            NumberOfDaysOfTrendingGames = numberOfDaysOfTrendingGames;
        }

        public int NumberOfTrendingGamesToShow { get; private set; }
        public int NumberOfDaysOfTrendingGames { get; private set; }

        protected bool Equals(TrendingGamesRequest other)
        {
            return NumberOfTrendingGamesToShow == other.NumberOfTrendingGamesToShow && NumberOfDaysOfTrendingGames == other.NumberOfDaysOfTrendingGames;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((TrendingGamesRequest)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (NumberOfTrendingGamesToShow * 397) ^ NumberOfDaysOfTrendingGames;
            }
        }
    }
}
