namespace BusinessLogic.Caching
{
    public interface ICacheable<in TInput, out TOutput>
    {
        int GetCacheExpirationInSeconds();
        string GetCacheKey(TInput inputParameter);
        TOutput GetFromSource(TInput inputParameter);
        TOutput GetResults(TInput inputParameter);
    }
}