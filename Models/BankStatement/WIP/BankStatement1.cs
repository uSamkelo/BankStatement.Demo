using System.Text.Json.Serialization;

namespace BankStatement.Demo.Models.BankStatement.WIP
{
    public class BankStatement1
    {
        [JsonPropertyName("statement_period")]
        public StatementPeriod StatementPeriod { get; set; }

        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("transactions")]
        public List<FlatTransaction> Transactions { get; set; }
    }

    public class StatementPeriod
    {
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }
    }

    public class FlatTransaction
    {
        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("post_date")]
        public DateTime? PostDate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("balance_after")]
        public decimal BalanceAfter { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
