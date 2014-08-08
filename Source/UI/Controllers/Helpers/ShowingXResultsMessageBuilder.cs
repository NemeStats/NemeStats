using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Controllers.Helpers
{
    public interface ShowingXResultsMessageBuilder
    {
        string BuildMessage(int maxNumberOfSearchResults, int actualNumberOfSearchResults);
    }
}
