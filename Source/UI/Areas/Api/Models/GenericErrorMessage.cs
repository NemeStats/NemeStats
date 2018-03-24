using System.Linq;

namespace UI.Areas.Api.Models
{
    public class GenericErrorMessage
    {
        public GenericErrorMessage(string errorMessage, int? errorSubCode = null)
        {
            Message = errorMessage;
            ErrorSubCode = errorSubCode;
        }

        public string Message { get; }
        public int? ErrorSubCode { get; }
    }
}