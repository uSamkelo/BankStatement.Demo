using BankStatement.Demo.Entities;

namespace BankStatement.Demo.Interfaces
{
    public interface IBankRepository
    {
        void AddBankStatement(BankStatementEntity bankStatement);
        Task<BankStatementEntity?> GetBankStatementByIdAsync(Guid id, int userId);

        // Used only by the signed-link redemption flow, where ownership was already
        // validated when the link was generated (GetPdfLinkToken) and proven by the signature.
        Task<BankStatementEntity?> GetBankStatementByIdAsync(Guid id);
    }
}