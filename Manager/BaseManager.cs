using Dal;
using System.Linq;
using Infrastructure.Model.Common;
using Microsoft.AspNetCore.Http;
using MRDbIdentity.Infrastructure.Interface;
using MRDbIdentity.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using MRDbIdentity.Domain;
using System.Threading.Tasks;
using Infrastructure.Entities;

namespace Manager
{

    public class BaseManager
    {
        protected readonly AppUserManager _appUserManager;
        protected readonly IUserRepository<AppUser> _userRepository;
        protected readonly IRoleRepository _roleRepository;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected string _currentUserEmail => _httpContextAccessor.HttpContext.User?.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;
        protected List<string> _currentUserRoles => _httpContextAccessor.HttpContext.User?.FindAll(ClaimsIdentity.DefaultRoleClaimType)?.Select(x => x.Value).ToList() ?? new List<string>();

        protected User _currentUser { get; set; }
        protected async Task<User> GetCurrentUser()
        {
            if (_currentUser == null)
            {
                var email = _currentUserEmail;
                if (string.IsNullOrWhiteSpace(email)) return null;

                _currentUser = await _appUserManager.FindByEmailAsync(email);
            }

            return _currentUser;
        }

        public BaseManager(IHttpContextAccessor httpContextAccessor, AppUserManager appUserManager, IUserRepository<AppUser> userRepository, IRoleRepository roleRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _appUserManager = appUserManager;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public ApiResponse Ok(object response = null, string message = null)
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message,
                Response = response,
                Error = null
            };
        }

        public ApiResponse Fail(string message = null, object error = null)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Error = error,
                Message = message,
                Response = null
            };
        }
    }
}
