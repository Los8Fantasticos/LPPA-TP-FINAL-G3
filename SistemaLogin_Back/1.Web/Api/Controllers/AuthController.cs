﻿using Api.Request;
using AutoMapper;
using Core.Business.Services;
using Core.Contracts.Data;
using Core.Contracts.Services;
using Core.Domain.ApplicationModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Transversal.Extensions;

namespace Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly IUsersPrivilegesService _privilegesService;
        private readonly IUsersService _usersService;
        private readonly ActionLoggerMiddlewareConfiguration _actionLoggerMiddlewareConfiguration;

        public AuthController(
            IMapper mapper,
            ILogger<AuthController> logger,
            IUsersService usersService,
            IUsersPrivilegesService privilegesService,
            ActionLoggerMiddlewareConfiguration actionLoggerMiddlewareConfiguration)
        {
            _mapper = mapper;
            _logger = logger;
            _usersService = usersService;
            _privilegesService = privilegesService;
            _actionLoggerMiddlewareConfiguration = actionLoggerMiddlewareConfiguration;

        }


        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            try
            {
                Users user = _mapper.Map<Users>(registerRequest);
                var result = await _usersService.CreateUserAsync(user, registerRequest.Password);
                if (!result.Data.IsRegistred)
                {
                    _logger.LogInformation("Failed to create new user {errors}", result.Errors.ToJson());

                    if (result.Data.Code.Any(e => (e == "DuplicateUserName" || e == "DuplicateEmail")))
                    {
                        return Conflict(new { Message = "Duplicated User Or Duplicated Email", Errors = ModelState.SerializeErrors() });
                    }

                    return Conflict(new { Message = "User Registration Failed", Errors = ModelState.SerializeErrors() });
                }


                _logger.LogInformation($"New user with UserName: {registerRequest.FirstName + string.Empty + registerRequest.LastName} has been registered succesfully.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering new user.");
                return BadRequest(ex.Message);
            }
        }

        //create login endpoint
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                var result = await _usersService.LoginUserAsync(loginRequest.username, loginRequest.password);

                if (!result.Success)
                {
                    switch (result.Errors.First())
                    {
                        case "InvalidCredentials":
                            return BadRequest(new { Message = "El usuario y/o contraseña es incorrecto" });
                        case "TokenError":
                            return StatusCode(500, "Bearer token couldn't be created");
                        case "UserNotExist":
                            return BadRequest(new { Message = "El usuario no existe" });
                        case "UserNotConfirmed":
                            return BadRequest(new { Message = "El usuario no confirmó su email" });
                    }
                }

                _logger.LogInformation("{userName} has logged in", result.Data.UserName);

                if (result.Issues.Count > 0)
                {
                    _logger.LogInformation("{ServiceName}.{MethodName} failed. {Errors}",
                        nameof(RefreshTokenService),
                        result.Issues);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging in.");
                return BadRequest(ex.Message);
            }

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                {
                    ModelState.AddModelError(string.Empty, $"{nameof(userId)} and {nameof(token)} are required.");
                    return BadRequest(ModelState.SerializeErrors());
                }

                var result = await _usersService.ConfirmUserEmailAsync(userId, token);
                if (!result.Succeeded)
                {
                    _logger.LogInformation("Email confirmation failed: {errors}", result.Errors.ToJson());
                    ModelState.AddIdentityResultErrors(result);
                    return BadRequest(new { Message = "Email confirmation failed.", Errors = ModelState.SerializeErrors() });
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        #region Helpers

        #endregion
    }
}