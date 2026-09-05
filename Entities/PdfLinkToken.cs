namespace BankStatement.Demo.Entities
{
    public class PdfLinkToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Signature { get; set; } = null!;

        public Guid BankStatementId { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? UsedAt { get; set; }

    }
}
