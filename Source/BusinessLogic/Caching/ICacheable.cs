namespace BusinessLogic.Caching
{
    public interface ICacheable<in TInput, out TOutput>
    {
        TOutput GetResults(TInput inputParameter);
    }
}