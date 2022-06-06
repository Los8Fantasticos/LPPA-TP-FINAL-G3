using AutoMapper;
using Core.Contracts.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class AuthController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly ActionLoggerMiddlewareConfiguration _actionLoggerMiddlewareConfiguration;

        public AuthController(
            IMapper mapper,
            ILogger<AuthController> logger,
            ActionLoggerMiddlewareConfiguration actionLoggerMiddlewareConfiguration
            )
        {
            _mapper = mapper;
            _logger = logger;
            _actionLoggerMiddlewareConfiguration = actionLoggerMiddlewareConfiguration;
        }

        
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string UserName)
        {
            _logger.LogInformation($"New user with UserName: {UserName} has been registered succesfully.");
            
            return null;
        }




    }
}
