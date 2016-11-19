namespace BusinessLogic.Logic
{
    public interface ITransformer
    {
        TDestination Transform<TDestination>(object source);
    }
}
