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
    public class UsersPrivilegesService : GenericService<UsersPrivileges>, IUsersPrivilegesService
    {
        public UsersPrivilegesService(IUnitOfWork unitOfWork) 
            : base(unitOfWork, unitOfWork.GetRepository<IUsersPrivilegesRepository>())
        {
            
        }
    }
}
