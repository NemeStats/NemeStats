using System.Linq;

namespace UI.Areas.Api.Models
{
    public class GenericErrorMessage
    {
        public GenericErrorMessage(string errorMessage)
        {
            Message = errorMessage;
        }

        public string Message { get; private set; }
    }
}