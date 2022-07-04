using Api.Request;
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
                var result = await _usersService.LoginUserAsync(loginRequest.username,loginRequest.password);

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
    }
}