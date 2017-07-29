using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LicenseExtractor.Interfaces;
using LicenseExtractor.Models;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace LicenseExtractor.NuGetResolver
{
    public class NugetDependencyResolver : IDependencyResolver
    {
        public async Task<IEnumerable<Package>> FetchMultipleAsync(IEnumerable<(string packageName, string version)> packageNames)
        {
            var requests = packageNames.Select(x => FetchAsync(x));
            await Task.WhenAll(requests);

            return requests.Select(x => x.Result);
        }

        public async Task<Package> FetchAsync((string packageName, string version) packageInfo)
        {
            var logger = new NugetNullLogger();
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = new SourceRepository(packageSource, providers);
            var packageMetadataResource = 
            await sourceRepository.GetResourceAsync<PackageMetadataResource>();
            var searchMetadata = await packageMetadataResource.GetMetadataAsync(
                new PackageIdentity(packageInfo.packageName, new NuGetVersion(packageInfo.version)),
                logger,
                CancellationToken.None);

            return new Package();
        }

//  TODO: WTF should I do with this?
        public class NugetNullLogger : NuGet.Common.ILogger
        {
            public void LogDebug(string data) => System.Console.WriteLine($"DEBUG: {data}");
            public void LogVerbose(string data) => System.Console.WriteLine($"VERBOSE: {data}");
            public void LogInformation(string data) => System.Console.WriteLine($"INFORMATION: {data}");
            public void LogMinimal(string data) => System.Console.WriteLine($"MINIMAL: {data}");
            public void LogWarning(string data) => System.Console.WriteLine($"WARNING: {data}");
            public void LogError(string data) => System.Console.WriteLine($"ERROR: {data}");
            public void LogSummary(string data) => System.Console.WriteLine($"SUMMARY: {data}");

            public void LogInformationSummary(string data)
            {
                throw new NotImplementedException();
            }

            public void LogErrorSummary(string data)
            {
                throw new NotImplementedException();
            }
        }
    }
}