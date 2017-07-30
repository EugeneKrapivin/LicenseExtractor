using System.Collections.Generic;
using System.Threading.Tasks;
using LicenseExtractor.Models;

namespace LicenseExtractor.Interfaces
{
    public interface IDependencyFetcher
    {
        ValueTask<IEnumerable<(string packageName, string version)>> FetchPackagesAsync(string path);
    }
}
