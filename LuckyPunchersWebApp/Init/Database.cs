using Dal;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MRDbIdentity.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZBApi.Init
{
    public static class Database
    {
        private static List<Role> Roles = new List<Role>
        {
            new Role
            {
                Name = "ADMIN",
            },

            new Role
            {
                Name = "USER",
            }
        };

        private static Dictionary<string, AppUser> Users = new Dictionary<string, AppUser>
        {
            {
                "Tf27324_()_",
                new AppUser
                {
                    Birthday = new DateTime(1995, 3, 20, 0, 0, 0, 0, DateTimeKind.Local),
                    FirstName = "Oleh",
                    LastName = "Tymofieiev",
                    Email = "oleg.timofeev20@gmail.com",
                    UserName = "somemyname",
                    Tels = new List<UserTel>
                    {
                        new UserTel
                        {
                            CreatedTime = DateTime.UtcNow,
                            Name = "Main",
                            Number = "+380508837161"
                        }
                    },
                    State = true,
                    Avatar = new UserAvatar
                    {
                        Src = "http://null"
                    }
                }
            }
        };

        public static async Task Seed(IServiceProvider service)
        {
            var userRepository = service.GetRequiredService<IUserStore<AppUser>>();
            var roleRepository = service.GetRequiredService<IRoleStore<Role>>();
            var userManager = service.GetRequiredService<AppUserManager>();

            Console.WriteLine($"Start seed database");

            foreach (var role in Roles)
            {
                var roleId = await roleRepository.GetRoleIdAsync(role, new System.Threading.CancellationToken());
                if (!string.IsNullOrWhiteSpace(roleId))
                {
                    role.Id = roleId;
                }
                else
                {
                    Console.WriteLine($"Add role {role.Name}");
                    await roleRepository.CreateAsync(role, new System.Threading.CancellationToken());
                }
            }

            foreach(var user in Users)
            {
                if((await userManager.FindByEmailAsync(user.Value.Email)) == null)
                {
                    var result = await userManager.CreateAsync(user.Value);
                    if (!result.Succeeded)
                    {
                        Console.WriteLine("Error");
                        Console.WriteLine(string.Join("\n", result.Errors));
                    }

                    result = await userManager.AddPasswordAsync(user.Value, user.Key);

                    await userManager.AddToRolesAsync(user.Value, Roles.Select(x => x.Name));
                }
            }

            Console.WriteLine($"Seed database finished");
        }
    }
}
