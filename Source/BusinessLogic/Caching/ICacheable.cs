namespace BusinessLogic.Caching
{
    public interface ICacheable<in TInput, out TOutput>
    {
        int GetCacheExpirationInSeconds();
        string GetCacheKey();
        TOutput GetFromSource(TInput inputParameter);
        TOutput GetResults(TInput inputParameter);
    }
}