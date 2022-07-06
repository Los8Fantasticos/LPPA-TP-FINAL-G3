using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transversal.Helpers.ResultClasses;
using Transversal.Helpers.JWT;
using Core.Domain.Enum;
using Core.Domain.ApplicationModels;
using Core.Domain.DTOs;

namespace Core.Business.Services
{
    public class UsersService : GenericService<Users>, IUsersService
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;

        private readonly IJwtBearerTokenHelper _jwtBearerTokenHelper;
        private readonly ITokenGenerator _refreshTokenFactory;
        private readonly IRefreshTokenService _refreshTokenService;

        public UsersService(
            IUnitOfWork unitOfWork,
            SignInManager<Users> signInManager,
            UserManager<Users> userManager,
            IJwtBearerTokenHelper jwtBearerTokenHelper,
            ITokenGenerator tokenGenerator,
            IRefreshTokenService refreshTokenService)
            : base(unitOfWork, unitOfWork.GetRepository<IUsersRepository>())
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtBearerTokenHelper = jwtBearerTokenHelper;
            _refreshTokenFactory = tokenGenerator;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<bool> CreateUserAsync(Users user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    string Errores = string.Join("\n", result.Errors.Select(p => p.Description));      
                    throw new Exception(Errores);
                }
                var role = PrivilegeEnum.User.ToString();
                result = await _userManager.AddToRoleAsync(user, role.ToUpper());
                if (!result.Succeeded)
                {
                    //_logger.LogInformation("Failed to assign role to new user {error}.", result.Errors.ToJson());
                    await _userManager.DeleteAsync(await _userManager.FindByNameAsync(user.UserName));
                    //_logger.LogInformation("{userName} has been deleted.", user.UserName);

                    //return BadRequest(new { Message = "User Role Assignment Failed", Errors = ModelState.SerializeErrors() });
                    return false;
                }
                return true;
            }
            catch (InvalidOperationException ex) //Si cae a esta exepción es porque no existe el rol user en la base...
            {
                await _userManager.DeleteAsync(await _userManager.FindByNameAsync(user.UserName));
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _userManager.DeleteAsync(new Users { Id = id });
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }
            return true;
        }

        public async Task<List<Users>> GetUsersAsync()
        {
            return (await _repository.Get()).ToList();            
        }       
        

        public async Task<IGenericResult<LoginTokenDto>> LoginUserAsync(string email, string password)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            
            if (identityUser is null) return new GenericResult<LoginTokenDto>("UserNotExist");
            
            if (identityUser.EmailConfirmed /*&& identityUser.Active //Mas adelante agregar active*/)
            {
                var result = await _userManager.CheckPasswordAsync(identityUser, password);
                return result ? await GenerateLoginToken(identityUser) : new GenericResult<LoginTokenDto>("InvalidCredentials");
            }
            else
                return new GenericResult<LoginTokenDto>("UserNotConfirmed");    
        }
        
        private async Task<IGenericResult<LoginTokenDto>> GenerateLoginToken(Users user)
        {
            var result = new GenericResult<LoginTokenDto>();

            var role = await _userManager.GetRolesAsync(user);

            var bearerToken = _jwtBearerTokenHelper.CreateJwtToken(user.Id, user.UserName, role.ToList());
            if (bearerToken is null)
            {
                result.AddError("TokenError");
                return result;
            }

            var refreshToken = _refreshTokenFactory.GenerateToken();
            var creationResult = await _refreshTokenService.CreateAsync(user.Id, refreshToken);

            if (!creationResult.Success)
            {
                result.Issues = creationResult.Errors;
            }

            var response = new LoginTokenDto
            {
                Token = bearerToken,
                RefreshToken = creationResult.Success ? refreshToken : null,
                ValidFrom = _jwtBearerTokenHelper.GetValidFromDate(bearerToken),
                ExpirationDate = _jwtBearerTokenHelper.GetExpirationDate(bearerToken),
                UserName = user.UserName,
                Email = user.Email
            };

            result.Data = response;
            return result;
        }


    }
}
