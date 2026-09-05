using AutoMapper;
using BankStatement.Demo.Entities;
using BankStatement.Demo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankStatement.Demo.Data
{
    public class BankRepository(AppDbContext context /*, IMapper mapper*/) : IBankRepository
    {
        public void AddBankStatement(BankStatementEntity bankStatement)
        {
            context.BankStatements.Add(bankStatement);
        }

        public async Task<BankStatementEntity?> GetBankStatementByIdAsync(Guid id)
        {
            return await context.BankStatements
                .Include(b => b.Transactions)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
