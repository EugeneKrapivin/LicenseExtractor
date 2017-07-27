using System.Collections.Generic;
using System.Threading.Tasks;
using LicenseExtractor.Models;

namespace LicenseExtractor.Interfaces
{
    public interface IReportGenerator
    {
        Task<Report> CreateReport(IEnumerable<Package> dependencies);
    }
}
