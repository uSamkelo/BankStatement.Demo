using BankStatement.Demo.Data;
using BankStatement.Demo.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BankStatement.Demo.Services
{
    public class PdfLinkService
    {
        private readonly byte[] bytes;
        private readonly AppDbContext context;

        public PdfLinkService(IConfiguration config, AppDbContext context)
        {
            var secret = config["FileSigningKey"] ?? "YourSuperSecretBackupKeyThatIsVeryLong888";
            bytes = Encoding.UTF8.GetBytes(secret);
            this.context = context;
        }

        public string GenerateSecureQueryParams(string fileName, TimeSpan validaty)
        {
            long expiresAt = DateTimeOffset.UtcNow.Add(validaty).ToUnixTimeSeconds();

            string payload = $"{fileName}:{expiresAt}";

            using var hmac = new HMACSHA256(bytes);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            string signature = WebEncoders.Base64UrlEncode(hash);

            return $"fileName={Uri.EscapeDataString(fileName)}&expiresAt={expiresAt}&signature={Uri.EscapeDataString(signature)}";
        }

        // Registers the link as "issued" so it can later be checked/consumed for single-use enforcement
        public async Task RegisterLinkAsync(string fileName, string signature, DateTimeOffset expiresAt)
        {
            if (!Guid.TryParse(fileName, out var statementId))
                return;

            context.PdfLinkTokens.Add(new PdfLinkToken
            {
                Signature = signature,
                BankStatementId = statementId,
                ExpiresAt = expiresAt
            });

            await context.SaveChangesAsync();
        }

        public bool IsLinkValid(string fileName, long expiresAt, string signature)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiresAt)
                return false;

            string payload = $"{fileName}:{expiresAt}";
            using var hmac = new HMACSHA256(bytes);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            string expectedSignature = WebEncoders.Base64UrlEncode(hash);

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(signature),
                Encoding.UTF8.GetBytes(expectedSignature)
            );
        }

        //single-use enforcement layer — validates signature/expiry AND checks/marks DB usage
        public async Task<bool> TryConsumeLinkAsync(string fileName, long expiresAt, string signature)
        {
            if (!IsLinkValid(fileName, expiresAt, signature))
                return false;

            var token = await context.PdfLinkTokens
                .FirstOrDefaultAsync(t => t.Signature == signature);

            // If no record found, the link wasn't issued via GetPdfLinkToken (or DB was reset) — reject to be safe
            if (token is null)
                return false;

            if (token.UsedAt is not null)
                return false; // already used

            if (token.ExpiresAt < DateTimeOffset.UtcNow)
                return false; // expired per DB record too

            token.UsedAt = DateTimeOffset.UtcNow;
            await context.SaveChangesAsync();

            return true;
        }
    }
}