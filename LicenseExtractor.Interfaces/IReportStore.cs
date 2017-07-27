using System.Threading.Tasks;

using LicenseExtractor.Models;

namespace LicenseExtractor.Interfaces
{
    public interface IReportStore
    {
        Task<bool> StoreAsync(Report report);
    }
}
