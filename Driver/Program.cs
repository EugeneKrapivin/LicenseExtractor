using System.Threading.Tasks;
using LicenseExtractor.NuGetResolver;

namespace LicenseExtractor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var resolver = new NugetDependencyFetcher();
            await resolver.FetchPackagesAsync("packages.config");
        }
    }
}
