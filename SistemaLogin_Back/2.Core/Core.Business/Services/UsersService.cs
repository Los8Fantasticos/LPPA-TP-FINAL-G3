using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.DTO;
using Microsoft.AspNetCore.Identity;
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
        public UsersService(IUnitOfWork unitOfWork, IGenericRepository<Users> repository, SignInManager<Users> signInManager, UserManager<Users> userManager)
            : base(unitOfWork, repository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task RegisterUser()
        {
            var user = _userManager.FindByNameAsync(userName).Result;
            if (user != null)
            {
                var result = _signInManager.CheckPasswordSignInAsync(user, password, false).Result;
                if (result.Succeeded)
                {
                    _signInManager.SignInAsync(user, false).Wait();
                }
            }
        }
    }
}
