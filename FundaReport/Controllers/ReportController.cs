using FundaReport.Models;
using FundaReport.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FundaReport.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet(Name = nameof(GenerateMakelaarReport))]
        public async Task<IActionResult> GenerateMakelaarReport()
        {
            MakelaarReportModel result;

            try
            {
                result = await _reportService.GetStandardMakelaarReportAsync();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, ex.Message);
            }

            return Ok(result);
        }

        [HttpPost(Name = nameof(GetMakelaarReportForQuery))]
        public async Task<IActionResult> GetMakelaarReportForQuery([FromBody] string query)
        {
            MakelaarReportModel result;

            try
            {
                result = await _reportService.GetMakelaarReportForQueryAsync([query]);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, ex.Message);
            }

            return Ok(result);
        }

    }
}