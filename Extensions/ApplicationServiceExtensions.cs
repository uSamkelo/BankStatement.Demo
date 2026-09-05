using BankStatement.Demo.Data;
using BankStatement.Demo.Interfaces;
using BankStatement.Demo.Mapping;
using BankStatement.Demo.Services;
using Microsoft.EntityFrameworkCore;

namespace BankStatement.Demo.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(opt =>

                opt.UseNpgsql(config.GetConnectionString("DefaultConnection"),
                o => o.ConfigureDataSource(dataSourceBuilder =>
                {
                    //dataSourceBuilder.UseClientCertificate();
                    dataSourceBuilder.EnableDynamicJson();
                })
            ));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<PdfLinkService>();
            services.AddAutoMapper(cfg => cfg.AddProfile<BankStatementProfile>());

            return services;
        }
    }
}
