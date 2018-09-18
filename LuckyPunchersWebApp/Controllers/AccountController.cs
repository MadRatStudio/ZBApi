﻿using Infrastructure.Model.User;
using Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZBApi.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        protected AccountManager _accountManager;

        public AccountController(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        [Route("anonimus")]
        public IActionResult Anonimus()
        {
            return Ok("Anonimusu");
        }

        [Route("auth")]
        [Authorize]
        public IActionResult Auth()
        {
            return Ok("Some secret data");
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            await _accountManager.SignIn(model);
            return Ok("Ok");
        }
    }
}
