
using BTPayPro.CPMPay.Reports;
using BTPayPro.CPMPay.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BTPayPro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CPMPayController : ControllerBase
    {
        private readonly CPMPayParser _parsingService;
        private readonly CPMPayReportGenerator _reportGenerator;
        public CPMPayController(CPMPayParser parsingService, CPMPayReportGenerator reportGenerator)
        {
            _parsingService = parsingService;
            _reportGenerator = reportGenerator;
        }

        [HttpPost("ParseCpmpayFile")]
        public async Task<IActionResult> ParseCpmpayFile(IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position to the beginning
                try
                {
                    var cpmpayFile = _parsingService.ParseCpmFile(stream, ct);
                    //var accountingReport = _reportGenerator.GenerateReport(cpmpayFile.Detail);
                    var csv = _reportGenerator.GenerateMarkdownReports(cpmpayFile.Detail);
                    return File(Encoding.UTF8.GetBytes(csv), "text/csv", "CPMPay_report.csv");
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }
}
