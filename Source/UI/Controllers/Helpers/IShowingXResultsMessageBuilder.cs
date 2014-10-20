using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Controllers.Helpers
{
    public interface IShowingXResultsMessageBuilder
    {
        string BuildMessage(int maxNumberOfSearchResults, int actualNumberOfSearchResults);
    }
}
