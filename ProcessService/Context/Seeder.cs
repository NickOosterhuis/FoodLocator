using System;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProcessService.Entities;

namespace ProcessService.Context
{
    public static class Seeder
    {
        public const string AdminId = SharedConstraints.AdminId;
        public const string AdminRoleId = SharedConstraints.AdminRoleId;
        public const string AdminProfileId = SharedConstraints.AdminProfileId;

        public const string RestaurantOwnerRoleId = SharedConstraints.RestaurantOwnerRoleId;
        public const string ConsumerRoleId = SharedConstraints.ConsumerRoleId;

        public static void SeedUsers(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            AddRoles(roleManager);
            AddSuperUser(userManager);
        }

        public static void Seed(this ModelBuilder builder)
        {
            SeedProfiles(builder);            
        }

        private static void SeedProfiles(ModelBuilder builder)
        {
            var adminProfile = new Profile()
            {
                Id = AdminProfileId,
                Name = "FoodLocator"
            };           

            builder.Entity<Profile>().HasData(
                adminProfile                
            );
        }

        private static void AddRoles(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = new IdentityRole { Id = AdminRoleId, Name = "Admin" };
            var restaurantOwnerRole = new IdentityRole { Id = RestaurantOwnerRoleId, Name = "RestaurantOwner" };
            var consumerRole = new IdentityRole { Id = ConsumerRoleId, Name = "Consumer" };

            if (!roleManager.RoleExistsAsync("Admin").Result)
                roleManager.CreateAsync(adminRole).Wait();

            if (!roleManager.RoleExistsAsync("RestaurantOwner").Result)
                roleManager.CreateAsync(restaurantOwnerRole).Wait();

            if (!roleManager.RoleExistsAsync("Consumer").Result)
                roleManager.CreateAsync(consumerRole).Wait();            
        }

        public static void AddSuperUser(UserManager<ApplicationUser> userManager)
        {
            var superUser = new ApplicationUser()
            {
                Id = AdminId,
                Email = "admin@foodlocator.com",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "admin@foodlocator.com",
                ProfileId = AdminProfileId
            };

            if (userManager.FindByEmailAsync(superUser.Email).Result == null)
            {
                IdentityResult result = userManager.CreateAsync(superUser, "Foodlocator2019!@").Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(superUser, "Admin").Wait();
            }
        }

    }
}
