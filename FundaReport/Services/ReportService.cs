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
            return await GetMakelaarReportForQueryAsync(_makelaarReportSettings.Queries);
        }

        public async Task<MakelaarReportModel> GetMakelaarReportForQueryAsync(string[] queries)
        { 
            _report = new MakelaarReportModel
            {
                MakelaarTables = new List<MakelaarTableModel>()
            };

            for (var tableIndex = 0; tableIndex < queries.Length; tableIndex++)
            {
                var query = queries[tableIndex];
                _report.MakelaarTables.Add(new MakelaarTableModel
                {
                    Query = query,
                    Rows = new List<MakelaarRowModel>()
                });

                var table = _report.MakelaarTables[tableIndex];
                var page = 1;
                var total = 0;
                _apiRequestCounter = 0;
                _stopwatch = new Stopwatch();
                _stopwatch.Start();

                while (page == 1 || page * _pageSize < total)
                { 
                    var fundaResponse = await _fundaHttpService.GetQueryResultsAsync(query, page, _pageSize);
                    _apiRequestCounter++;
                    if (fundaResponse.IsFailed)
                    {
                        // if Funda API call fails, give it a little time and try again
                        Thread.Sleep(3000);
                        _report.MakelaarTables[tableIndex].TotalTimeWaitingOnRateLimit += 3;
                        checkForRateLimiting(tableIndex);
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

                    checkForRateLimiting(tableIndex);
                }
                _stopwatch.Stop();
                UpdateQueryTableCompilationStats(tableIndex);
                table.Rows = table.Rows.OrderByDescending(x => x.Total).Take(10).ToList();
            }
            return _report;
        }

        private void checkForRateLimiting(int tableIndex)
        {
            // check if we need to wait before making more API calls
            if (_apiRequestCounter >= 100 && _stopwatch.ElapsedMilliseconds < 60000)
            {
                var sleepTime = 61000 - (int)_stopwatch.ElapsedMilliseconds;
                _report.MakelaarTables[tableIndex].TotalTimeWaitingOnRateLimit += sleepTime / 1000;
                Thread.Sleep(sleepTime);
                ResetRateLimitObservers(tableIndex);
            }
            else if (_stopwatch.ElapsedMilliseconds >= 60000)
            {
                ResetRateLimitObservers(tableIndex);
            }
        }

        private void ResetRateLimitObservers(int currentTableIndex)
        {
            UpdateQueryTableCompilationStats(currentTableIndex);
            _apiRequestCounter = 0;
            _stopwatch.Reset();
        }
        
        private void UpdateQueryTableCompilationStats(int currentTableIndex)
        {
            var elapsedSeconds = _stopwatch.ElapsedMilliseconds / 1000;
            _report.MakelaarTables[currentTableIndex].TotalTimePreparingTable += (int)elapsedSeconds;
            _report.MakelaarTables[currentTableIndex].NumberOfApiRequests += _apiRequestCounter;
        }
    }
}
