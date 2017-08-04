namespace LicenseExtractor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using LicenseExtractor.Models;
    using LicenseExtractor.NuGetResolver;
    using LicenseExtractor.PaketResolver;
    using Microsoft.Extensions.Logging;

    public class Driver
    {
        private ILogger<Driver> _logger;
        private string[] knownTargets = new [] { "paket.dependencies","paket.lock", "packages.config", "*.csproj" };
        
        public Driver(ILogger<Driver> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunAsync(Options options)
        {
            var targets = GetTargets(options);

            if (!targets.Any())
            {
                _logger.LogWarning($"No targets were found.");
                return;
            }
            
            _logger.LogInformation($"Found {targets.Count()} targets.");
            
            var dict = new Dictionary<string, IEnumerable<Package>>();
            
            
            foreach (var target in targets)
            {
                if (!File.Exists(target))
                {
                    System.Console.WriteLine($"{target} not found");
                    continue;
                }
                var fetcher = new PaketDependencyFetcher();
                var packages = await fetcher.FetchPackagesAsync(target);
                var resolver = new NugetDependencyResolver();
                var res = await resolver.FetchMultipleAsync(packages.Where(x => !x.packageName.StartsWith("System") && !x.packageName.Contains("gigya") && !x.packageName.Contains("Gigya")));

                // TODO: var tName = Regex.Match(target, pattern).Groups["target"].Value;
                // TODO: dict[tName] = res.ToList();
            }

            var inverse = dict.Values
                .SelectMany(packages => packages)
                .GroupBy(package => package.Name)
                .Select(package => new
                {
                    Package = package.First(),
                    UsedBy = string.Join(", ", dict.Keys.Where(key => dict[key].SingleOrDefault(x => x.Name.Equals(package.Key)) != null))
                });

            var sumBuilder = new StringBuilder().AppendLine("name|version|usedBy|authors|license|package site");
            var sumString = inverse.Aggregate(sumBuilder, (sb, r) => sb.AppendLine($"{r.Package.Name}|{r.Package.Version}|{r.UsedBy}|{string.Join(", ", r.Package.Authors)}|{r.Package.License}|{r.Package.PackageSite}"));
            await File.WriteAllTextAsync($"sum.csv", sumString.ToString());
        }

        private string[] GetTargets(Options options)
        {
            string[] targets;
            if (!string.IsNullOrEmpty(options.FilePath))
            {
                if (!File.Exists(options.FilePath)) throw new ArgumentException($"File {options.FilePath} doesn't exist.");

                targets = new[] { options.FilePath };
            }
            else
            {
                var path = options.DirectoryPath;

                if (!string.IsNullOrEmpty(options.DirectoryPath))
                {
                    if (!Directory.Exists(options.DirectoryPath))
                    {
                        _logger.LogWarning($"Directory {options.DirectoryPath} was not found, using current as default.");
                        path = Directory.GetCurrentDirectory();
                    }
                }

                if (!knownTargets.Contains(options.Target))
                {
                    _logger.LogWarning($"Unkown target {options.Target}, using paket.lock as default.");
                    options.Target = "paket.lock";
                }

                targets = Directory.GetFiles(path, options.Target, SearchOption.AllDirectories);
            }

            return targets;
        }
    }
}