using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class UsersService : GenericService<Users>, IUsersService
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        public UsersService(IUnitOfWork unitOfWork, SignInManager<Users> signInManager, UserManager<Users> userManager)
            : base(unitOfWork, unitOfWork.GetRepository<IUsersRepository>())
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<bool> CreateUserAsync(Users users, string password)
        {
            var result = await _userManager.CreateAsync(users, password);
            if(!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }
            return true;
        }

        public async Task<bool> LoginUserAsync(string email, string password)
        {       
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser != null && identityUser.EmailConfirmed)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(identityUser, password, false);
                if (result.Succeeded)
                {
                    return true;
                }
                else
                    return false;
            }
                return false;
        }
    }
}
