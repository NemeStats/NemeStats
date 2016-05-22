namespace UI.Mappers
{
    public interface IMapperService<in TSource, out TResult> where TSource: class  
    {
        TResult Map(TSource source);
    }
}