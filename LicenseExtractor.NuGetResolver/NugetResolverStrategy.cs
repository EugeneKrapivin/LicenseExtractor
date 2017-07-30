using System;
using LicenseExtractor.Interfaces;

namespace LicenseExtractor.NuGetResolver
{
    public class NugetResolverStrategy : IResolverStrategy
    {
        public IDependencyFetcher GetFetcher() => new NugetDependencyFetcher();
        public IDependencyResolver GetResolver() => new NugetDependencyResolver();
    }
}