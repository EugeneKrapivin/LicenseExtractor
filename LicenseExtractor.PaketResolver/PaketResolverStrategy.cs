using System;
using LicenseExtractor.Interfaces;
using LicenseExtractor.NuGetResolver;

namespace LicenseExtractor.PaketResolver
{
    public class PaketResolverStrategy : IResolverStrategy
    {
        public IDependencyFetcher GetFetcher()
        {
            return new PaketDependencyFetcher();
        }

        public IDependencyResolver GetResolver()
        {
            return new NugetDependencyResolver();
        }
    }
}