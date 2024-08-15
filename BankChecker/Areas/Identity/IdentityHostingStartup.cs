using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Data;

[assembly: HostingStartup(typeof(BankChecker.Areas.Identity.IdentityHostingStartup))]
namespace BankChecker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AppDbIdentity>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AppDbIdentityConnection"),
                        sqlOptions => sqlOptions.MigrationsAssembly("BankChecker")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<AppDbIdentity>();
            });
        }
    }
}