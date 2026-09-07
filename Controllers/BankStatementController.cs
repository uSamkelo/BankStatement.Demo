using AutoMapper;
using BankStatement.Demo.DTOs;
using BankStatement.Demo.Entities;
using BankStatement.Demo.Extensions;
using BankStatement.Demo.Interfaces;
using BankStatement.Demo.Models;
using BankStatement.Demo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;

namespace BankStatement.Demo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BankStatementController(IUnitOfWork unitOfWork, PdfLinkService pdfLinkService, IMapper mapper) : ControllerBase
    {
        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("{id:guid}/pdf")]
        public async Task<IActionResult> GetPdf(Guid id)
        {
            var bank = await unitOfWork.BankRepository.GetBankStatementByIdAsync(id, CurrentUserId);
            if (bank is null)
                return NotFound();

            var bankStatementDto = mapper.Map<BankStatementDto>(bank);
            var pdf = BankStatementPdfExtensions.GeneratePdf(bankStatementDto);
            return File(pdf, "application/pdf", "bankstatement.pdf");
        }

        [HttpPost("create")]
        public async Task<ActionResult<BankStatementDto>> CreateBankStatement(BankStatementDto bankStatementDto)
        {
            var bank = mapper.Map<BankStatementEntity>(bankStatementDto);
            bank.UserId = CurrentUserId;

            unitOfWork.BankRepository.AddBankStatement(bank);
            await unitOfWork.CompleteAsync();

            return Ok(bankStatementDto);
        }

        // Step 1: authenticated caller requests a shareable, expiring link for a statement's PDF
        [HttpGet("{id:guid}/GetPdfLinkToken")]
        public async Task<IActionResult> GetPdfLinkToken(Guid id)
        {
            var bank = await unitOfWork.BankRepository.GetBankStatementByIdAsync(id, CurrentUserId);
            if (bank is null)
                return NotFound();

            var validity = TimeSpan.FromMinutes(15);
            var queryParams = pdfLinkService.GenerateSecureQueryParams(id.ToString(), validity);

            var parsed = QueryHelpers.ParseQuery(queryParams);
            var signature = parsed["signature"].ToString();
            var expiresAt = DateTimeOffset.UtcNow.Add(validity);

            await pdfLinkService.RegisterLinkAsync(id.ToString(), signature, expiresAt);

            var url = $"{Request.Scheme}://{Request.Host}/api/BankStatement/pdf?{queryParams}";
            return Ok(new { url });
        }

        // Step 2: public, anonymous redemption endpoint — validates signature + expiry, then streams the PDF
        [AllowAnonymous]
        [HttpGet("pdf")]
        public async Task<IActionResult> GetPdfByLink([FromQuery] string fileName, [FromQuery] long expiresAt, [FromQuery] string signature)
        {
            if (!await pdfLinkService.TryConsumeLinkAsync(fileName, expiresAt, signature))
                return Unauthorized("Link is invalid, expired, or already used.");

            if (!Guid.TryParse(fileName, out var statementId))
                return BadRequest("Invalid statement identifier.");

            var bank = await unitOfWork.BankRepository.GetBankStatementByIdAsync(statementId);
            if (bank is null)
                return NotFound();

            var bankStatementDto = mapper.Map<BankStatementDto>(bank);
            var pdf = BankStatementPdfExtensions.GeneratePdf(bankStatementDto);
            return File(pdf, "application/pdf", "bankstatement.pdf");
        }
    }
}