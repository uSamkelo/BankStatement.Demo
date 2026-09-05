namespace BankStatement.Demo.Interfaces
{
    public interface IUnitOfWork
    {
        IBankRepository BankRepository { get; }

        Task CompleteAsync();
    }
}
