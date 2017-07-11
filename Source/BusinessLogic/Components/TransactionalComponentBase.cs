using System;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;

namespace BusinessLogic.Components
{
    public abstract class TransactionalComponentBase<TInput, TOutput> : DbComponentBase<TInput, TOutput>
    {
        internal virtual Action PostExecuteAction { get; set; }

        public virtual TOutput ExecuteTransaction(TInput inputParameter, ApplicationUser currentUser, IDataContext dataContext)
        {
            if (dataContext.CurrentTransaction() != null)
            {
                return Execute(inputParameter, currentUser, dataContext);
            }

            using (var transaction = dataContext.BeginTransaction())
            {
                try
                {
                    var result = Execute(inputParameter, currentUser, dataContext);
                    transaction.Commit();
                    PostExecuteAction?.Invoke();

                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public override TOutput Execute(TInput inputParameter, ApplicationUser currentUser)
        {
            //TODO how to use the IContainer here instead?
            using (var dataContext = new NemeStatsDataContext())
            {
                return ExecuteTransaction(inputParameter, currentUser, dataContext);
            }
        }
    }
}
