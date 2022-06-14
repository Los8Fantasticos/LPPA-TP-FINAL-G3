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
    }
}
