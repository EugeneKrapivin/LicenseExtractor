using System;
using LicenseExtractor.Interfaces;

namespace LicenseExtractor.NuGetResolverStrategy
{
    public class NugetResolverFactory : IResolverStrategy
    {
        public IDependencyFetcher GetFetcher()
        {
            throw new NotImplementedException();
        }

        public IDependencyResolver GetResolver()
        {
            throw new NotImplementedException();
        }
    }
}