using FundaReport.Models;
using FundaReport.Settings;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace FundaReport.Services
{
    public class ReportService
    {
        private readonly FundaHttpService _fundaHttpService;
        private readonly MakelaarReportSettings _makelaarReportSettings;
        private readonly int _pageSize = 25;
        private Stopwatch _stopwatch;
        private int _apiRequestCounter;

        public ReportService(IOptions<AppSettings> appSettings, FundaHttpService fundaHttpService) 
        {
            _fundaHttpService = fundaHttpService;
            _makelaarReportSettings = appSettings.Value.MakelaarReportSettings;
        }

        public async Task<MakelaarReportModel> GenerateMakelaarReportAsync()
        {
            var report = new MakelaarReportModel
            {
                MakelaarTables = new List<MakelaarTableModel>()
            };

            for (var queryIndex = 0; queryIndex < _makelaarReportSettings.Queries.Length; queryIndex++)
            {
                var query = _makelaarReportSettings.Queries[queryIndex];
                report.MakelaarTables.Add(new MakelaarTableModel
                {
                    Query = query,
                    Rows = new List<MakelaarRowModel>()
                });

                var table = report.MakelaarTables[queryIndex];
                var page = 1;
                var total = 26;
                _apiRequestCounter = 0;
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
                while (page * _pageSize < total)
                { 
                    var fundaResponse = await _fundaHttpService.GetQueryResults(query, page, _pageSize);
                    _apiRequestCounter++;
                    var currentPage = fundaResponse.Content;
                    total = currentPage.TotaalAantalObjecten;

                    foreach (var listing in currentPage.Objects)
                    {
                        var makelaar = table.Rows.FirstOrDefault(x => x.MakelaarId == listing.MakelaarId);
                        if (makelaar != null)
                        {
                            makelaar.Total++;
                        }
                        else
                        {
                            table.Rows.Add(new MakelaarRowModel
                            {
                                MakelaarNaam = listing.MakelaarNaam,
                                MakelaarId = listing.MakelaarId,
                                Total = 1
                            });
                        }
                    }

                    page++;

                    if (_apiRequestCounter >= 100 && _stopwatch.ElapsedMilliseconds < 60000)
                    {
                        Thread.Sleep(60000 - (int)_stopwatch.ElapsedMilliseconds);
                        ResetRateLimitObservers();
                    }
                    else if (_stopwatch.ElapsedMilliseconds >= 60000)
                    {
                        ResetRateLimitObservers();
                    }
                }

                _stopwatch.Stop();
                table.Rows = table.Rows.OrderByDescending(x => x.Total).Take(10).ToList();
            }
            return report;
        }

        private void ResetRateLimitObservers()
        {
            _apiRequestCounter = 0;
            _stopwatch.Reset();
        }
    }
}
