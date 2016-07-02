namespace UI.Mappers.Interfaces
{
    public interface IMapperFactory
    {
        ICustomMapper<TSource, TResult> GetMapper<TSource, TResult>() where TSource : class;
    }
}