using RollbarSharp;

namespace BusinessLogic.Jobs
{
    public abstract class BaseJobService
    {
        protected readonly IRollbarClient RollbarClient;

        protected BaseJobService(IRollbarClient rollbar)
        {
            RollbarClient = rollbar;
        }
    }
}