namespace BankStatement.Demo.Models.BankStatement
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime PostDate { get; set; }
        public string Description { get; set; }
        public Category Categories { get; set; }
        public Type Type { get; set; }
        public double Amount { get; set; }
        public double BalanceAfter { get; set; }

        //could be converted to enum
        public string Status { get; set; }

    }

    //WIP
    public enum Category
    {
        Income,
        Debit
    }
    //
    public enum Type
    {
        Credit,
        Debit
    }
}
