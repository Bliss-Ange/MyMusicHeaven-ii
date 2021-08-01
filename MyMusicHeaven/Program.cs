using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyMusicHeaven.Data; //so that can find the context class to check the table connection
using MyMusicHeaven.Models;
using Microsoft.AspNetCore.Identity;
using MyMusicHeaven.Areas.Identity.Data;

namespace MyMusicHeaven
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //call the intialize function (ProductData.cs)
            using (var scope = host.Services.CreateScope()) 
            {
                var services = scope.ServiceProvider;
                try 
                {
                    var context = services.GetRequiredService<MyMusicHeavenNewContext>();
                    context.Database.Migrate();
                    ProductData.Initialize(services);
                    var userManager = services.GetRequiredService<UserManager<MyMusicHeavenUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await ContextRoles.SeedRolesAsync(userManager, roleManager);
                }
                catch(Exception ex) 
                {
                    var logger = services.GetRequiredService<ILogger<Program>>(); //log to display error message at cmd
                    logger.LogError(ex, "An error occurred seeding the DB."); 
                }
            }
                host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
