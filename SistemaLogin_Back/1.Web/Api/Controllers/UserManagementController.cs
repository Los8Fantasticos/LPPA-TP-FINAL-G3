using AutoMapper;
using Core.Contracts.Data;
using Core.Contracts.Services;
using Core.Domain.DTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementController> _logger;
        private readonly IUsersPrivilegesService _userPrivilegesService;
        private readonly IUsersService _usersService;
        private readonly ActionLoggerMiddlewareConfiguration _actionLoggerMiddlewareConfiguration;

        public UserManagementController(IMapper mapper, ILogger<UserManagementController> logger, IUsersPrivilegesService userPrivilegesService, IUsersService usersService, ActionLoggerMiddlewareConfiguration actionLoggerMiddlewareConfiguration)
        {
            _mapper = mapper;
            _logger = logger;
            _userPrivilegesService = userPrivilegesService;
            _usersService = usersService;
            _actionLoggerMiddlewareConfiguration = actionLoggerMiddlewareConfiguration;
        }

    }
}
