namespace UI.Transformations
{
    public interface ITransformer
    {
        TDestination Transform<TDestination>(object source);
    }
}
