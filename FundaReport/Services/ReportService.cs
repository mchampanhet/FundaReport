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
        // 25 appears to be the max pagesize the API supports
        private readonly int _pageSize = 25;
        private Stopwatch _stopwatch;
        private int _apiRequestCounter;
        private MakelaarReportModel _report;

        public ReportService(IOptions<AppSettings> appSettings, FundaHttpService fundaHttpService) 
        {
            _fundaHttpService = fundaHttpService;
            _makelaarReportSettings = appSettings.Value.MakelaarReportSettings;
        }

        public async Task<MakelaarReportModel> GetStandardMakelaarReportAsync()
        {
            var report = new MakelaarReportModel
            {
                MakelaarTables = new List<MakelaarTableModel>()
            };

            foreach (var query in _makelaarReportSettings.Queries)
            {
                report.MakelaarTables.Add(await GetMakelaarReportForQueryAsync(query));
            }

            return report;
        }

        public async Task<MakelaarTableModel> GetMakelaarReportForQueryAsync(string query)
        {
            var table = new MakelaarTableModel
            {
                Query = query,
                Rows = new List<MakelaarRowModel>()
            };

            var page = 1;
            var total = 0;
            _apiRequestCounter = 0;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            // TODO: find a more robust method for rate-limiting outgoing API calls
            // in order to be able to multithread the requests to Funda API
            while (page == 1 || page * _pageSize < total)
            { 
                var fundaResponse = await _fundaHttpService.GetQueryResultsAsync(query, page, _pageSize);
                _apiRequestCounter++;
                if (fundaResponse.IsFailed)
                {
                    // if Funda API call fails despite our internal rate-limiting, give it a little time and try again
                    Thread.Sleep(3000);
                    table.TotalTimeWaitingOnRateLimit += 3;
                    checkForRateLimiting(table);
                    fundaResponse = await _fundaHttpService.GetQueryResultsAsync(query, page, _pageSize);
                    _apiRequestCounter++;

                    if (fundaResponse.IsFailed)
                    {
                        // if Funda API still fails at this point, abandon report and throw exception
                        var message = fundaResponse.Error ?? "Funda API not responding";
                        throw new Exception(message);
                    }
                }

                var currentPage = fundaResponse.Content;
                total = currentPage.TotaalAantalObjecten;

                foreach (var listing in currentPage.Objects)
                {
                    // find out if we've already saved this makelaar before
                    var makelaar = table.Rows.FirstOrDefault(x => x.MakelaarId == listing.MakelaarId);
                    if (makelaar != null)
                    {
                        // increment the existing makelaar's property count
                        makelaar.Total++;
                    }
                    else
                    {
                        // add the new makelaar
                        table.Rows.Add(new MakelaarRowModel
                        {
                            MakelaarNaam = listing.MakelaarNaam,
                            MakelaarId = listing.MakelaarId,
                            Total = 1
                        });
                    }
                }

                page++;

                checkForRateLimiting(table);
            }
            _stopwatch.Stop();
            UpdateQueryTableCompilationStats(table);
            table.Rows = table.Rows.OrderByDescending(x => x.Total).Take(10).ToList();
            return table;
        }

        private void checkForRateLimiting(MakelaarTableModel table)
        {
            // check if we need to wait before making more API calls
            if (_apiRequestCounter >= 100 && _stopwatch.ElapsedMilliseconds < 60000)
            {
                var sleepTime = 61000 - (int)_stopwatch.ElapsedMilliseconds;
                table.TotalTimeWaitingOnRateLimit += sleepTime / 1000;
                Thread.Sleep(sleepTime);
                ResetRateLimitObservers(table);
            }
            else if (_stopwatch.ElapsedMilliseconds >= 60000)
            {
                ResetRateLimitObservers(table);
            }
        }

        private void ResetRateLimitObservers(MakelaarTableModel table)
        {
            UpdateQueryTableCompilationStats(table);
            _apiRequestCounter = 0;
            _stopwatch.Reset();
        }
        
        private void UpdateQueryTableCompilationStats(MakelaarTableModel table)
        {
            var elapsedSeconds = _stopwatch.ElapsedMilliseconds / 1000;
            table.TotalTimePreparingTable += (int)elapsedSeconds;
            table.NumberOfApiRequests += _apiRequestCounter;
        }
    }
}
