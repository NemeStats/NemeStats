using System;
using System.Linq;

namespace BusinessLogic.Models.Validation
{
    public abstract class BaseValidatableRequestSaver
    {
        public void ValidateRequest<T>(T request) where T : IValidatableRequest
        {
            if (request == null)
            {
                throw new ArgumentNullException(typeof(T).ToString());
            }
            var validationErrors = request.ValidateRequest();
            if (validationErrors.Any())
            {
                throw new ArgumentException(typeof(T).ToString() + " : " + validationErrors.First().ErrorMessage);
            }

        }



    }
}