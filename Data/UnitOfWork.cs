using AutoMapper;
using BankStatement.Demo.Interfaces;
using System.Threading.Tasks;

namespace BankStatement.Demo.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public AppDbContext _context { get; }
        //private readonly IMapper _mapper;

        public UnitOfWork(AppDbContext context)//, IMapper mapper)
        {
            this._context = context;
            //this._mapper = mapper;
        }

        public IBankRepository BankRepository => new BankRepository(_context /*, _mapper*/);

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
