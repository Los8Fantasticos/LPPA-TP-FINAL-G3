using AutoMapper;
using Core.Contracts.Data;
using Core.Contracts.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Request;
using Api.Request.Privileges;
using Core.Domain.ApplicationModels;

namespace Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]

    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementController> _logger;
        private readonly IUsersPrivilegesService _userPrivilegesService;
        private readonly IUsersService _usersService;
        private readonly IPrivilegesService _privilegesService;

        public UsersController(
            IMapper mapper,
            ILogger<UserManagementController> logger,
            IUsersPrivilegesService userPrivilegesService,
            IUsersService usersService,
            IPrivilegesService privilegesService)
        {
            _mapper = mapper;
            _logger = logger;
            _userPrivilegesService = userPrivilegesService;
            _usersService = usersService;
            _privilegesService = privilegesService;
        }

        [HttpDelete]
        [Authorize(Roles = "Administrador")]
        //[AllowAnonymous]
        public async Task<IActionResult> DeleteUser([FromBody] string id)
        {
            try
            {
                var result = await _usersService.DeleteUserAsync(id);
                if (!result)
                {
                    return Problem("Error al eliminar el usuario.");
                }
                _logger.LogInformation($"Deleted privilege: {id} succesfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteUser" + ex.Message);
                return Problem(ex.Message);
            }
        }
        
        //edit user endpoint
        [HttpPut]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditUser(EditUserRequest user)
        {
            try
            {
                Users editedUser = _mapper.Map<Users>(user);
                editedUser.Active = true;
                var result = await _usersService.UpdateUserAsync(editedUser);
                if (!result)
                {
                    return Problem("Error al editar el usuario.");
                }
                _logger.LogInformation($"Edited user: {user.Id} succesfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in EditUser" + ex.Message);
                return Problem(ex.Message);
            }
        }
    }
}
