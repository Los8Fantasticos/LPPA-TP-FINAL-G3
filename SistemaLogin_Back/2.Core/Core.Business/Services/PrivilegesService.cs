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
        public PrivilegesService(IUnitOfWork unitOfWork) 
            : base(unitOfWork, unitOfWork.GetRepository<IPrivilegesRepository>())
        {
            
        }

        public async Task<bool> AddPrivilege(Privileges privilege)
        {
            if (CreateAsync(privilege).IsCompletedSuccessfully)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        

    }
}
