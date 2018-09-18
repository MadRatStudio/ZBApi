using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dal;
using Infrastructure.Entities;
using Infrastructure.Model.Common;
using Infrastructure.Model.User;
using Manager.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MRDbIdentity.Infrastructure.Interface;
using MRDbIdentity.Service;

namespace Manager
{
    public class UserManager : BaseManager
    {
        public UserManager(IHttpContextAccessor httpContextAccessor, AppUserManager appUserManager, IUserRepository<AppUser> userRepository, IRoleRepository roleRepository) : base(httpContextAccessor, appUserManager, userRepository, roleRepository)
        {
        }

        public async Task<ApiResponse> TokenEmail(UserLoginModel model)
        {
            if (model == null) return Fail();
            var identity = await GetIdentity(model);

            if (identity == null) return Fail();

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                expires: now.Add(TimeSpan.FromSeconds(AuthOptions.LIFETIME)),
                claims: identity.Claims,
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encoded = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Ok(encoded, "Success");
        }

        protected async Task<ClaimsIdentity> GetIdentity(UserLoginModel model)
        {
            var user = await _appUserManager.FindByEmailAsync(model.Email);
            if (user == null) return null;

            if (!await _appUserManager.CheckPasswordAsync(user, model.Password)) return null;
            var roles = await _appUserManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
            }

            ClaimsIdentity identity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return identity;
        }
    }
}
