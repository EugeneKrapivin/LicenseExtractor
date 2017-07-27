using System.Collections.Generic;
using System.Threading.Tasks;
using LicenseExtractor.Models;

namespace LicenseExtractor.Interfaces
{
    public interface IDependencyFetcher
    {
        Task<IEnumerable<Package>> FetchPackagesAsync(string path);
    }
}
