using System;
using System.Collections.Generic;

namespace LicenseExtractor.Models
{
    public class Package
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public IEnumerable<Maintainer> Owner { get; set; }
        public Uri PackageSite { get; set; }
        public string License { get; set; }
    }
}
