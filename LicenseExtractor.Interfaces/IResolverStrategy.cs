namespace LicenseExtractor.Interfaces
{
    public interface IResolverStrategy
    {
        IDependencyFetcher GetFetcher();
        IDependencyResolver GetResolver();
    }
}
