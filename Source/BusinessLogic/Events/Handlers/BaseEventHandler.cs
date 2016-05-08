using BusinessLogic.DataAccess;

namespace BusinessLogic.Events.Handlers
{
    public abstract class BaseEventHandler
    {
        protected readonly IDataContext DataContext;

        protected BaseEventHandler(IDataContext dataContext)
        {
            DataContext = dataContext;
        }
    }
}