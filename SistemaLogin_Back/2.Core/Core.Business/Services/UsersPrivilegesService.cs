﻿using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.DTO;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class UsersPrivilegesService : GenericService<UsersPrivileges>, IUsersPrivilegesService
    {
        private readonly UserManager<UsersPrivileges> _userManager;
        private readonly RoleManager<Privileges> _roleManager;
        public UsersPrivilegesService(IUnitOfWork unitOfWork, UserManager<UsersPrivileges> userManager, RoleManager<Privileges> roleManager) 
            : base(unitOfWork, unitOfWork.GetRepository<IUsersPrivilegesRepository>())
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }       
        
        public async Task<bool> AssignPrivilegesToUser(string userId, IEnumerable<string> privileges)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var currentPrivileges = await _userManager.GetRolesAsync(user);
            foreach (var privilege in privileges)
            {
                if(!await _roleManager.RoleExistsAsync(privilege))
                {
                    return false; //se le puede devolver un mensaje tipo "Error: El privilegio '*' con id '*' no existe."
                }
            }

            IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, currentPrivileges);
            if (!removeResult.Succeeded)
            {
                return false; //mensaje tipo "Error al remover privilegios."
            }
            IdentityResult addResult = await _userManager.AddToRolesAsync(user, privileges);
            if (!addResult.Succeeded)
            {
                return false; //mensaje tipo "Error al agregar privilegios."
            }

            return true;
        }
        
    }
}
