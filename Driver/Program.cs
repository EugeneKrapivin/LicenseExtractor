using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicenseExtractor.NuGetResolver;
using LicenseExtractor.PaketResolver;
using System.Text;
using System.IO;
using System;
using LicenseExtractor.Interfaces;
using LicenseExtractor.Models;
using System.Text.RegularExpressions;
using CommandLine;

namespace LicenseExtractor
{
    partial class Program
    {

        // public static string[] targets = 
        // {
        //     @"D:\dvlp\Services\AccountsService\paket.lock",
        //     @"D:\dvlp\Services\CertificateProvisionService\paket.lock",
        //     @"D:\dvlp\Services\Common\paket.lock",
        //     @"D:\dvlp\Services\CountersService\paket.lock",
        //     @"D:\dvlp\Services\EmailService\paket.lock",
        //     @"D:\dvlp\Services\EventService\paket.lock",
        //     @"D:\dvlp\Services\Gator\paket.lock",
        //     @"D:\dvlp\Services\GS_Soa\paket.lock",
        //     @"D:\dvlp\Services\IDXService\paket.lock",
        //     @"D:\dvlp\Services\ImageService\paket.lock",
        //     @"D:\dvlp\Services\ImportFriendsService\paket.lock",
        //     @"D:\dvlp\Services\IndexCreationService\paket.lock",
        //     @"D:\dvlp\Services\Infrastructure\paket.lock",
        //     @"D:\dvlp\Services\JWTService\paket.lock",
        //     @"D:\dvlp\Services\KeyEncryptionService\paket.lock",
        //     @"D:\dvlp\Services\KeyManagementService\paket.lock",
        //     @"D:\dvlp\Services\microdot\paket.lock",
        //     @"D:\dvlp\Services\NotificationsService\paket.lock",
        //     @"D:\dvlp\Services\OpenIdProviderService\paket.lock",
        //     @"D:\dvlp\Services\OpenIdRPService\paket.lock",
        //     @"D:\dvlp\Services\PartnersService\paket.lock",
        //     @"D:\dvlp\Services\PasswordService\paket.lock",
        //     @"D:\dvlp\Services\PolicyService\paket.lock",
        //     @"D:\dvlp\Services\RBAService\paket.lock",
        //     @"D:\dvlp\Services\ReportsService\paket.lock",
        //     @"D:\dvlp\Services\RequestService\paket.lock",
        //     @"D:\dvlp\Services\SamlSpService\paket.lock",
        //     @"D:\dvlp\Services\SchemaService\paket.lock",
        //     @"D:\dvlp\Services\ScreenSetsService\paket.lock",
        //     @"D:\dvlp\Services\SessionService\paket.lock",
        //     @"D:\dvlp\Services\SiteConfigService\paket.lock",
        //     @"D:\dvlp\Services\SiteGroupService\paket.lock",
        //     @"D:\dvlp\Services\SitesService\paket.lock",
        //     @"D:\dvlp\Services\SocialService\paket.lock",
        //     @"D:\dvlp\Services\SubscriptionService\paket.lock",
        //     @"D:\dvlp\Services\TFAProvidersService\paket.lock",
        //     @"D:\dvlp\Services\TFAService\paket.lock",
        //     @"D:\dvlp\Services\TokenService\paket.lock",
        //     @"D:\dvlp\Services\UserIdService\paket.lock",
        //     @"D:\dvlp\Services\UserManagement\paket.lock",
        //     @"D:\dvlp\Services\USMService"
        // };
        public static string[] knownTargets = new [] { "paket.dependencies","paket.lock", "packages.config", "*.csproj" };

        private async Task RunAsync(Options options)
        {
            string[] targets;
            if (!string.IsNullOrEmpty(options.FilePath))
            {
                if (!File.Exists(options.FilePath)) throw new ArgumentException($"File {options.FilePath} doesn't exist.");               

                targets = new [] { options.FilePath };
            } 
            else
            {
                var path = options.DirectoryPath;

                if (!string.IsNullOrEmpty(options.DirectoryPath))
                {
                    if (!Directory.Exists(options.DirectoryPath))
                    {
                        // TODO warn
                        path = Directory.GetCurrentDirectory();
                    }
                }

                if (!knownTargets.Contains(options.Target))
                {
                    // TODO: log warn
                    options.Target = "paket.lock";
                }

                targets = Directory.GetFiles(path, options.Target, SearchOption.AllDirectories);
            }
            var dict = new Dictionary<string, IEnumerable<Package>>();
            const string pattern = @"D:\\dvlp\\Services\\(?<target>.*)\\paket\.dependencies";
            System.Console.WriteLine($"Found {targets.Count()} targets");
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
                
            var sumBuilder = new StringBuilder().AppendLine("name|version|usedBy|authors|license|package site");
            var sumString = inverse.Aggregate(sumBuilder,(sb, r) => sb.AppendLine($"{r.Package.Name}|{r.Package.Version}|{r.UsedBy}|{string.Join(", ", r.Package.Authors)}|{r.Package.License}|{r.Package.PackageSite}"));
            await File.WriteAllTextAsync($"sum.csv", sumString.ToString());
        }

        static async Task Main(string[] args)
        {
            await new Program().RunAsync(new Options{
                DirectoryPath = @"D:\dvlp\Services",
                Target = "paket.dependencies"
            });
            // var parsed = GetArgumentsParser().ParseArguments<Options>(args);

            // switch (parsed)
            // {
            //     case Parsed<Options> opts:
            //         {
            //             await new Program().RunAsync(opts.Value);
            //             break;
            //         }
            //     case NotParsed<Options> opts:
            //         {
            //             System.Console.WriteLine("damn");
            //             break;
            //         }
            //     default:
            //         throw new Exception("WAT?");
            // }
        }

        private static Parser GetArgumentsParser() => new Parser((settings) =>
        {
            settings.CaseSensitive = false;
            settings.IgnoreUnknownArguments = true;
            settings.HelpWriter = Console.Out;
        });
    }
}
