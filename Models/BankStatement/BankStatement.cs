namespace BankStatement.Demo.Models.BankStatement
{
    public class BankStatements
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Account Accounts { get; set; }

        public Transaction Transaction { get; set; }
    }
}
