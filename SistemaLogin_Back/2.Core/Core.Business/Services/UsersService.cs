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
using AutoMapper;

namespace Core.Business.Services
{
    public class UsersService : GenericService<Users>, IUsersService
    {
        private readonly UserManager<Users> _userManager;

        private readonly IJwtBearerTokenHelper _jwtBearerTokenHelper;
        private readonly ITokenGenerator _refreshTokenFactory;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public UsersService(
            IUnitOfWork unitOfWork,
            UserManager<Users> userManager,
            IJwtBearerTokenHelper jwtBearerTokenHelper,
            ITokenGenerator tokenGenerator,
            IRefreshTokenService refreshTokenService,
            IMapper mapper,
            IEmailService emailService)
            : base(unitOfWork, unitOfWork.GetRepository<IUsersRepository>())
        {
            _userManager = userManager;
            _jwtBearerTokenHelper = jwtBearerTokenHelper;
            _refreshTokenFactory = tokenGenerator;
            _refreshTokenService = refreshTokenService;
            _emailService = emailService;
            _mapper = mapper;
        }
        
        public async Task<IGenericResult<RegisterDto>> CreateUserAsync(Users user, string password)
        {
            RegisterDto registerDto = new RegisterDto();
            GenericResult<RegisterDto> usuarioDto = new GenericResult<RegisterDto>();
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                usuarioDto = _mapper.Map<GenericResult<RegisterDto>>(result);
                if (!result.Succeeded)
                {
                    //string Errores = string.Join("\n", result.Errors.Select(p => p.Description));
                    return usuarioDto;
                }
                var role = PrivilegeEnum.User.ToString();
                result = await _userManager.AddToRoleAsync(user, role.ToUpper());
                if (!result.Succeeded)
                {
                    await _userManager.DeleteAsync(await _userManager.FindByNameAsync(user.UserName));
                    usuarioDto.Data = registerDto;
                    return usuarioDto;
                }
                
                await _emailService.RegistrationEmailAsync(user);
                registerDto.IsRegistred = true;
                usuarioDto.Data = registerDto;
                return usuarioDto;
            }
            catch (InvalidOperationException ex) when (ex is Exception) //Si cae a esta exepción es porque no existe el rol user en la base...
            {
                await _userManager.DeleteAsync(await _userManager.FindByNameAsync(user.UserName));
                usuarioDto.Data.IsRegistred = false;
                throw new Exception(ex.Message);
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

        public async Task<IdentityResult> ConfirmUserEmailAsync(string userId, string token)
        {
            var identityUser = await _userManager.FindByIdAsync(userId);
            return await _userManager.ConfirmEmailAsync(identityUser ?? throw new Exception("Ocurrió un error al confirmar el usuario"), token);
        }



        #region Helpers
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

        #endregion
    }
}
