using AutoMapper;
using Core.Contracts.Data;
using Core.Contracts.Services;
using Core.Domain.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly IPrivilegesService _privilegesService;
        private readonly ActionLoggerMiddlewareConfiguration _actionLoggerMiddlewareConfiguration;

        public UserManagementController(
            IMapper mapper,
            ILogger<UserManagementController> logger,
            IUsersPrivilegesService userPrivilegesService,
            IUsersService usersService,
            IPrivilegesService privilegesService,
            ActionLoggerMiddlewareConfiguration actionLoggerMiddlewareConfiguration)
        {
            _mapper = mapper;
            _logger = logger;
            _userPrivilegesService = userPrivilegesService;
            _usersService = usersService;
            _privilegesService = privilegesService;
            _actionLoggerMiddlewareConfiguration = actionLoggerMiddlewareConfiguration;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePrivilege(Privileges privileges)
        {
            var result = await _privilegesService.CreatePrivilegeAsync(privileges);
            if (!result)
            {
                return Problem("Error al crear el rol.");
            }
            return Ok();
        }



    }
}
