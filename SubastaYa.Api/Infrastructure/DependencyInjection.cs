using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<AuditService>();
            services.AddScoped<IBiddingService, BiddingService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IAuctionService, AuctionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<AuctionClosingService>();


            return services;
        }
    }
}
