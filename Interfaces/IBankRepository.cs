using BankStatement.Demo.Entities;

namespace BankStatement.Demo.Interfaces
{
    public interface IBankRepository
    {
        void AddBankStatement(BankStatementEntity bankStatement);
        Task<BankStatementEntity?> GetBankStatementByIdAsync(Guid id);
    }
}