using System.Collections.Generic;
using System.Threading.Tasks;
using LicenseExtractor.Models;

namespace LicenseExtractor.Interfaces
{
    public interface IDependencyResolver
    {
        Task<Package> FetchAsync((string packageName, string version) packageInfo);
        Task<IEnumerable<Package>> FetchMultipleAsync(IEnumerable<(string packageName, string version)> packages);
    }
}
