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
            var response = await _fundaHttpService.GetQueryResults(_makelaarReportSettings.Queries[0], 1, 100);
            var report = new MakelaarReportModel
            {
                MakelaarTables = new List<MakelaarTableModel>
                {
                    new MakelaarTableModel
                    {
                        Rows = new List<MakelaarRowModel>()
                    }
                }
            };

            foreach (var item in response.Objects)
            {
                report.MakelaarTables[0].Rows.Add(new MakelaarRowModel
                {
                    MakelaarNaam = item.MakelaarNaam,
                    MakelaarId = item.MakelaarId,
                    Total = 1
                });
            }
            return report;
        }
    }
}
