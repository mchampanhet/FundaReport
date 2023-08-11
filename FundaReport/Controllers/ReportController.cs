using FundaReport.Models;
using FundaReport.Services;
using Microsoft.AspNetCore.Mvc;

namespace FundaReport.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet(Name = nameof(GenerateMakelaarReport))]
        public Task<MakelaarReportModel> GenerateMakelaarReport()
        {
            var result = _reportService.GenerateMakelaarReportAsync();
            return result;
        }
    }
}