using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Threading.Tasks;
using LicenseExtractor.Interfaces;
using System.IO;
using System.Xml.Linq;
using LicenseExtractor.Models;

namespace LicenseExtractor.NuGetResolver
{
    public class NugetDependencyFetcher : IDependencyFetcher
    {
        public Task<IEnumerable<(string packageName, string version)>> FetchPackagesAsync(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new ArgumentException($"File {path} doesn't exist.");

            var text = File.ReadAllText(path);

            var document = XDocument.Parse(text);

            var packages = document.Element("packages")
                .Elements()
                .Select(x => (packageName: x.Attribute("id").Value, version: x.Attribute("version").Value));

            return Task.FromResult(packages);
        }
    }
}