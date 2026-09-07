namespace BankStatement.Demo.DTOs
{
    public class TransactionDto
    {
        public string ExternalTransactionId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string PaymentChannel { get; set; }
        public string Category { get; set; }

        public MerchantDto Merchant { get; set; }
    }
}
