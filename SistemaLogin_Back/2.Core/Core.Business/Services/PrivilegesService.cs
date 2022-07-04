﻿using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.ApplicationModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.Services
{
    public class PrivilegesService : GenericService<Privileges>, IPrivilegesService
    {
        private readonly RoleManager<Privileges> _roleManager;
        public PrivilegesService(IUnitOfWork unitOfWork, RoleManager<Privileges> roleManager)
            : base(unitOfWork, unitOfWork.GetRepository<IPrivilegesRepository>())
        {
            _roleManager = roleManager;
        }

        public async Task<bool> CreatePrivilegeAsync(Privileges privileges)
        {
            var result = await _roleManager.CreateAsync(privileges);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }
            return true;
        }

        public async Task<bool> DeletePrivilegeAsync(string id)
        {
            var result = await _roleManager.DeleteAsync(new Privileges { Id = id });
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }
            return true;
        }

        public async Task<Privileges> UpdatePrivilegeAsync(Privileges privileges)
        {
            var result = await _roleManager.UpdateAsync(privileges);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }
            return privileges;
        }

        public async Task<List<Privileges>> GetPrivilegesAsync()
        {
            return (await _repository.Get()).ToList();
        }

        public async Task<Privileges> GetPrivilegeByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }
    }
}
