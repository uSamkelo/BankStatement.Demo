namespace BankStatement.Demo.DTOs
{
    public class BankStatementDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string StatementId { get; set; }
        public string InstitutionName { get; set; }
        public string RoutingNumber { get; set; }

        public string AccountHolder { get; set; }
        public string AccountNumberMasked { get; set; }
        public string AccountType { get; set; }
        public string Currency { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal StartingBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }

        // Store category breakdown dynamically as JSONB in Postgres
        public Dictionary<string, decimal> CategoryBreakdown { get; set; }

        public List<TransactionDto> Transactions { get; set; } = new();
    }
}
