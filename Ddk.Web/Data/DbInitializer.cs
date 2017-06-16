using Ddk.Data;
using Ddk.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Linq;

namespace Smetko.Kitchen.Data
{
    public static class DbInitializer
    {
        public static async void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            context.Database.EnsureCreated();

            //if (context.Users.Any())
            //{
            //    return;
            //}

            var roles = new string[] { "User", "Admin" };
            foreach (var role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                await roleStore.CreateAsync(new IdentityRole()
                {
                    Name = role,
                    NormalizedName = role.ToLower()
                });

                var user = new ApplicationUser
                {
                    UserName = string.Concat(role, "@smetko.bg").ToLower(),
                    NormalizedUserName = string.Concat(role, "@smetko.bg").ToLower(),
                    Email = string.Concat(role, "@smetko.bg"),
                    NormalizedEmail = string.Concat(role, "@smetko.bg").ToLower(),
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var userStore = new UserStore<ApplicationUser>(context);
                await userStore.AddToRoleAsync(user, role.ToLower());
                await userManager.CreateAsync(user, "parola");
            }
        }
    }
}
