using FundaReport.Models;
using FundaReport.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        [HttpGet, Route("GetStandardMakelaarReport")]
        [SwaggerOperation(nameof(GetStandardMakelaarReport))]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MakelaarReportModel))]
        [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> GetStandardMakelaarReport()
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

        [HttpPost, Route("GetMakelaarReportForQuery")]
        [SwaggerOperation(nameof(GetMakelaarReportForQuery))]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MakelaarReportModel))]
        [SwaggerResponse((int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> GetMakelaarReportForQuery([FromBody] string query)
        {
            MakelaarReportModel response = new MakelaarReportModel
            {
                MakelaarTables = new List<MakelaarTableModel>()
            };

            try
            {
                var result = await _reportService.GetMakelaarReportForQueryAsync(query);
                response.MakelaarTables.Add(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, ex.Message);
            }

            return Ok(response);
        }

    }
}