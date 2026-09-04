using System.Text.Json.Serialization;

namespace BankStatement.Demo.Models.BankStatement.WIP
{
    public class BankStatement2
    {
        [JsonPropertyName("bank_metadata")]
        public BankMetadata BankMetadata { get; set; }

        [JsonPropertyName("account_details")]
        public AccountDetails AccountDetails { get; set; }

        [JsonPropertyName("statement_period")]
        public DetailedStatementPeriod StatementPeriod { get; set; }

        [JsonPropertyName("balance_summary")]
        public BalanceSummary BalanceSummary { get; set; }

        [JsonPropertyName("category_breakdown")]
        public Dictionary<string, decimal> CategoryBreakdown { get; set; }

        [JsonPropertyName("transactions")]
        public List<DetailedTransaction> Transactions { get; set; }
    }

    public class BankMetadata
    {
        [JsonPropertyName("institution_name")]
        public string InstitutionName { get; set; }

        [JsonPropertyName("routing_number")]
        public string RoutingNumber { get; set; }

        [JsonPropertyName("statement_id")]
        public string StatementId { get; set; }
    }

    public class AccountDetails
    {
        [JsonPropertyName("account_holder")]
        public string AccountHolder { get; set; }

        [JsonPropertyName("account_number_masked")]
        public string AccountNumberMasked { get; set; }

        [JsonPropertyName("account_type")]
        public string AccountType { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }

    public class DetailedStatementPeriod
    {
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("total_days")]
        public int TotalDays { get; set; }
    }

    public class BalanceSummary
    {
        [JsonPropertyName("starting_balance")]
        public decimal StartingBalance { get; set; }

        [JsonPropertyName("total_deposits")]
        public decimal TotalDeposits { get; set; }

        [JsonPropertyName("total_withdrawals")]
        public decimal TotalWithdrawals { get; set; }

        [JsonPropertyName("ending_balance")]
        public decimal EndingBalance { get; set; }

        [JsonPropertyName("average_daily_balance")]
        public decimal AverageDailyBalance { get; set; }
    }

    public class DetailedTransaction
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("merchant")]
        public MerchantInfo Merchant { get; set; }

        [JsonPropertyName("payment_channel")]
        public string PaymentChannel { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }

    public class MerchantInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("merchant_code")]
        public string MerchantCode { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }
}
