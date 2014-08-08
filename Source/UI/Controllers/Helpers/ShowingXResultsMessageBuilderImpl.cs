using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Controllers.Helpers
{
    public class ShowingXResultsMessageBuilderImpl : ShowingXResultsMessageBuilder
    {
        internal static readonly string RECENT_GAMES_MESSAGE_FORMAT = "(Last {0} Games)";

        public string BuildMessage(int maxNumberOfSearchResults, int actualNumberOfSearchResults)
        {
            if (actualNumberOfSearchResults < maxNumberOfSearchResults)
            {
                return string.Empty;
            }

            return string.Format(RECENT_GAMES_MESSAGE_FORMAT, actualNumberOfSearchResults);
        }
    }
}
