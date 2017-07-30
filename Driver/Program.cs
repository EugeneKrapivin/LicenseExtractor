using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicenseExtractor.NuGetResolver;
using LicenseExtractor.PaketResolver;
using CommandLine;
using System.Text;
using CommandLine.Text;
using System.IO;
using System;
using LicenseExtractor.Interfaces;
using LicenseExtractor.Models;
using System.Text.RegularExpressions;

namespace LicenseExtractor
{
    class Program
    {

        public static string[] targets = 
        {
            @"D:\dvlp\um\NotificationsService\paket.lock",
            @"D:\dvlp\um\RBAService\paket.lock",
            @"D:\dvlp\um\RBAService.Interface\paket.lock",
            @"D:\dvlp\um\PolicyService\paket.lock",
            @"D:\dvlp\um\PolicyService.Interface\paket.lock",
            @"D:\dvlp\um\TFAService\paket.lock",
            @"D:\dvlp\um\TFAService.Interface\paket.lock",
            @"D:\dvlp\um\TFAService\paket.lock",
            @"D:\dvlp\um\TFAService.Interface\paket.lock",
            @"D:\dvlp\um\SitesService\paket.lock",
            @"D:\dvlp\um\SitesService.Interface\paket.lock",
            @"D:\dvlp\um\USMService\paket.lock",
            @"D:\dvlp\um\USMService.Interface\paket.lock",
            @"D:\dvlp\um\PartnersService\paket.lock",
            @"D:\dvlp\um\PartnersService.Interface\paket.lock",
            @"D:\dvlp\um\IndexCreationService\paket.lock",
            @"D:\dvlp\um\TFAProvidersService\paket.lock",
            @"D:\dvlp\um\TFAProvidersService.Interface\paket.lock",
        };
        static async Task Main(string[] args)
        {
            var dict = new Dictionary<string, IEnumerable<Package>>();
            const string pattern = @"D:\\dvlp\\um\\(?<target>.*)\\paket\.lock";
            foreach (var target in targets)
            {
                if(!File.Exists(target))
                {
                    System.Console.WriteLine($"{target} not found");
                    continue;
                }
                var fetcher = new PaketDependencyFetcher();
                var packages = await fetcher.FetchPackagesAsync(target);
                var resolver = new NugetDependencyResolver();
                var res = await resolver.FetchMultipleAsync(packages.Where(x => !x.packageName.StartsWith("System") && !x.packageName.Contains("gigya") && !x.packageName.Contains("Gigya")));

                var tName = Regex.Match(target, pattern).Groups["target"].Value;
                dict[tName] = res.ToList();
            }
            
            var inverse = dict.Values
                .SelectMany(packages => packages)
                .GroupBy(package => package.Name)
                .Select(package => new 
                {
                    Package = package.First(), 
                    UsedBy = string.Join(", ", dict.Keys.Where(key => dict[key].SingleOrDefault(x => x.Name.Equals(package.Key)) != null))
                });
            
            foreach(var kv in dict)
            {
                var builder = new StringBuilder().AppendLine("name\\version\\authors\\license\\package site");
                var result = kv.Value.Aggregate(builder,(sb, r) => sb.AppendLine($"{r.Name}\\{r.Version}\\{string.Join(", ", r.Authors)}\\{r.License}\\{r.PackageSite}"));
                await File.WriteAllTextAsync($"{kv.Key}.csv", result.ToString());
            }

        
            var sumBuilder = new StringBuilder().AppendLine("name\\version\\usedBy\\authors\\license\\package site");
            var sumString = inverse.Aggregate(sumBuilder,(sb, r) => sb.AppendLine($"{r.Package.Name}\\{r.Package.Version}\\{r.UsedBy}\\{string.Join(", ", r.Package.Authors)}\\{r.Package.License}\\{r.Package.PackageSite}"));
            await File.WriteAllTextAsync($"sum.csv", sumString.ToString());
        }

        public static IResolverStrategy GetStrategy(string target)
        {
            switch(target)
            {
                case "paket.lock":
                    return new PaketResolverStrategy();
                case "packages.config":
                    return new NugetResolverStrategy();                    
            }
            throw new Exception("Do not support current target");
        }
    }
}
