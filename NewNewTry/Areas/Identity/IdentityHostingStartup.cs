using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewNewTry.Areas.Identity.Data;
using NewNewTry.Data;

[assembly: HostingStartup(typeof(NewNewTry.Areas.Identity.IdentityHostingStartup))]
namespace NewNewTry.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<NewNewTryContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("NewNewTryContextConnection")));

                services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<NewNewTryContext>();
            });
        }
    }
}