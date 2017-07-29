using System;
using System.Collections.Generic;

namespace LicenseExtractor.Models
{
    public class Package
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public IEnumerable<Maintainer> Owners { get; set; }
        public Uri PackageSite { get; set; }
        public Uri License { get; set; }
    }
}
