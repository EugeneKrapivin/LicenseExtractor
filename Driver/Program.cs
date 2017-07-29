using System.Linq;
using System.Threading.Tasks;
using LicenseExtractor.NuGetResolver;

namespace LicenseExtractor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fetcher = new NugetDependencyFetcher();
            var packages = await fetcher.FetchPackagesAsync("packages.config");
            var resolver = new NugetDependencyResolver();
            var res = await resolver.FetchMultipleAsync(packages);

        }
    }
}
