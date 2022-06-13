using AutoMapper;
using Core.Contracts.Data;
using Core.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController :ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly IPrivilegesService _privilegesService;
        private readonly IUsersService _usersService;
        private readonly ActionLoggerMiddlewareConfiguration _actionLoggerMiddlewareConfiguration;

        public UserManagementController(IMapper mapper, ILogger<AuthController> logger, IPrivilegesService privilegesService, IUsersService usersService, ActionLoggerMiddlewareConfiguration actionLoggerMiddlewareConfiguration)
        {
            _mapper = mapper;
            _logger = logger;
            _privilegesService = privilegesService;
            _usersService = usersService;
            _actionLoggerMiddlewareConfiguration = actionLoggerMiddlewareConfiguration;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get()
        {
            var users = await _usersService.GetAllAsync();
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(usersDto);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _usersService.GetByIdAsync(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<User>(userDto);
            await _usersService.CreateAsync(user);
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(string id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<User>(userDto);
            await _usersService.UpdateAsync(id, user);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            await _usersService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("{id}/privileges")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPrivileges(string id)
        {
            var privileges = await _privilegesService.GetAllAsync();
            var privilegesDto = _mapper.Map<IEnumerable<PrivilegeDto>>(privileges);
            return Ok(privilegesDto);
        }

        [HttpPost("{id}/privileges")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostPrivileges(string id, [FromBody] PrivilegeDto privilegeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var privilege = _mapper.Map<Privilege>(privilegeDto);
            await _privilegesService.CreateAsync(id, privilege);
            return Ok(privilege);
        }

        [HttpDelete("{id}/privileges/{privilegeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePrivileges(string id, string privilegeId)
        {
            await _privilegesService.DeleteAsync(id, privilegeId);
            return Ok();
        }
    }
}
