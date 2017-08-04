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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Reflection;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace LicenseExtractor
{
    public class Program
    {

        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(x =>
            {
                x.AddConsole();
                x.AddDebug();
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<Driver>().AsSelf();
            var container = builder.Build();

            try
            {
                GetArgumentsParser().ParseArguments<Options>(args)
                    .WithParsed(opts => container.Resolve<Driver>().RunAsync(opts).GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                container.Resolve<ILogger<Program>>().LogCritical(ex, "Somethings went terribly wrong");
            }
        }
        private static Parser GetArgumentsParser() => new Parser((settings) =>
        {
            settings.CaseSensitive = false;
            settings.IgnoreUnknownArguments = true;
            settings.HelpWriter = Console.Out;
        });
    }
}
