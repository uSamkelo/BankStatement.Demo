namespace BankStatement.Demo.Entities
{
    public class TransactionEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BankStatementId { get; set; }
        public BankStatementEntity BankStatement { get; set; }

        public string ExternalTransactionId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string PaymentChannel { get; set; }
        public string Category { get; set; }

        // Owned Entity mapped inline or stored as JSONB
        public MerchantEmbed Merchant { get; set; }
    }
}
