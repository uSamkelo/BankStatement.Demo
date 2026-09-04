using BankStatement.Demo.Data.Seeded;
using BankStatement.Demo.Models.BankStatement;
using BankStatement.Demo.Models.BankStatement.WIP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankStatement.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankStatementController : ControllerBase
    {
        [HttpGet("GetStatements")]
        public async Task<IActionResult> GetStatements()
        {
            var result = SeededData.GetBank1();

            return Ok(result);
        }

        [HttpPost("createpdf")]
        public IActionResult GetPdf(BankStatement2 bankStatement)
        {
            var pdf = BankStatementPdfExtensions.GeneratePdf(bankStatement);
            return File(pdf, "application/pdf", "bankstatement.pdf");
        }

    }
}
