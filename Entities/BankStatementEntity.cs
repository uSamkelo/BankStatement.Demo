namespace BankStatement.Demo.Entities
{
    public class BankStatementEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
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

        public List<TransactionEntity> Transactions { get; set; } = new();
    }
}
