namespace BusinessLogic.DataAccess.Security
{
    public interface ISecuredEntityValidatorFactory
    {
        ISecuredEntityValidator MakeSecuredEntityValidator<TEntity>(IDataContext dataContext) where TEntity : class;
    }
}