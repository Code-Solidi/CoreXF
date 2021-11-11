/*
 * Copyright (c) 2016-2021 Code Solidi Ltd. All rights reserved.
 * Licensed under the GNU GENERAL PUBLIC LICENSE Version 2. See GNU-GPL.txt in the project root for license information.
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;

namespace HostApp5.WebApi.Data
{
    public static class SeedDb
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<UsersDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            context.Database.EnsureCreated();
            if (!context.Users.Any())
            {
                var user = new IdentityUser
                {
                    Email = "test@gmail.com",
                    UserName = "test",
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                userManager.CreateAsync(user, "Test@123");
            }
        }
    }
}