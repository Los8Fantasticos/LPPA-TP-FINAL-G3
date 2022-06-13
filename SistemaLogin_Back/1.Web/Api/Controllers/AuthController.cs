using Api.Request;
using AutoMapper;
using Core.Contracts.Data;
using Core.Contracts.Services;
using Core.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
            ActionLoggerMiddlewareConfiguration actionLoggerMiddlewareConfiguration
            )
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
    }
}
