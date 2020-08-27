using Microsoft.AspNetCore.Identity;
using OA.Domain.Auth;
using OA.Persistence.Enum;
using System.Collections.Generic;

namespace OA.Persistence.Seeds
{
    public static class DefaultUser
    {
        public static List<ApplicationUser> IdentityBasicUserList()
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            return new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    Id = Constants.SuperAdminUser,
                    UserName = "superadmin",
                    Email = "superadmin@gmail.com",
                    FirstName = "Amit",
                    LastName = "Naik",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Password@123"),
                },
                new ApplicationUser
                {
                    Id = Constants.BasicUser,
                    UserName = "basicuser",
                    Email = "basicuser@gmail.com",
                    FirstName = "Basic",
                    LastName = "User",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Password@123"),

                },
            };
        }
    }
}
