using BTPayPro.Autmpay.Reports;
using BTPayPro.Autmpay.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BTPayPro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutmpayController : ControllerBase
    {
        private readonly AutmpayFileParser _parser;
        private readonly AccountingReportGenerator _reporter;

        public AutmpayController(AutmpayFileParser parser, AccountingReportGenerator reporter)
        {
            _parser = parser;
            _reporter = reporter;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            using var stream = file.OpenReadStream();
            var records = await _parser.ParseFile(stream, ct);
            var csv = _reporter.GenerateMarkdownReport(records.Details);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", "AutmpayReport.csv");
        }
    }
}
