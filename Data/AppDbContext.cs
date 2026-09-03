using BankStatement.Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace BankStatement.Demo.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
