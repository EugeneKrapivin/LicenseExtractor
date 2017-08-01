using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LicenseExtractor.Interfaces;
using Paket;

namespace LicenseExtractor.PaketResolver
{
    public class PaketDependencyFetcher : IDependencyFetcher
    {
        public ValueTask<IEnumerable<(string packageName, string version)>> FetchPackagesAsync(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new ArgumentException($"File {path} doesn't exist.");

            var lf = DependenciesFile.ReadFromFile(path);
            
            return new ValueTask<IEnumerable<(string packageName, string version)>>
                (lf.GetDependenciesInGroup(lf.Groups.First().Key).Select(x => (packageName: x.Key.ToString(), version: x.Value.ToString())));
        }
    }
}