using FundaReport.Models;
using FundaReport.Settings;
using Microsoft.Extensions.Options;

namespace FundaReport.Services
{
    public class ReportService
    {
        private readonly FundaHttpService _fundaHttpService;
        private readonly MakelaarReportSettings _makelaarReportSettings;
        public ReportService(IOptions<AppSettings> appSettings, FundaHttpService fundaHttpService) 
        {
            _fundaHttpService = fundaHttpService;
            _makelaarReportSettings = appSettings.Value.MakelaarReportSettings;
        }

        public async Task<MakelaarReportModel> GenerateMakelaarReportAsync()
        {
            await _fundaHttpService.GetQueryResults(_makelaarReportSettings.Queries[0], 1, 100);
            return null;
        }
    }
}
