using Health.Application.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application
{

    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<ApplicationRole> roleManager)
        {
            string[] roles = { "Admin", "Patient", "Doctor", "Nurse" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new ApplicationRole { Name = role });
                }
            }
        }

        public static async Task SeedAdminUserAsync(
        UserManager<User> userManager,
        RoleManager<ApplicationRole> roleManager)
        {
            
            const string adminEmail = "admin@health.com"; 
            const string adminPassword = "AdminPassword123!"; 

          
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
              
                adminUser = new User
                {
                    UserName = "AdminUser",
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true 
                };

                
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
              
            }
           
        }
    }


}
