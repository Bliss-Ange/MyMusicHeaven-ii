using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyMusicHeaven.Areas.Identity.Data;
using MyMusicHeaven.Data;

[assembly: HostingStartup(typeof(MyMusicHeaven.Areas.Identity.IdentityHostingStartup))]
namespace MyMusicHeaven.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MyMusicHeavenContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("MyMusicHeavenContextConnection")));

                services.AddDefaultIdentity<MyMusicHeavenUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<MyMusicHeavenContext>();
            });
        }
    }
}