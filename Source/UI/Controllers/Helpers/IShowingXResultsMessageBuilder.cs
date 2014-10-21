using System.Linq;

namespace UI.Controllers.Helpers
{
    public interface IShowingXResultsMessageBuilder
    {
        string BuildMessage(int maxNumberOfSearchResults, int actualNumberOfSearchResults);
    }
}
