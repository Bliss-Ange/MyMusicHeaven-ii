using Microsoft.AspNetCore.Identity;
using MyMusicHeaven.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMusicHeaven.Data
{
    public enum Roles
    {
        SuperAdmin,
        Admin,
        Customer,
        Staff
    }
    public class ContextRoles
    {

        public static async Task SeedRolesAsync(UserManager<MyMusicHeavenUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            //create role in dbo.AspNetRoles
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Customer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Staff.ToString()));
        }

    }
}
