using Core.Contracts.Repositories;
using Core.Domain.DTO;

using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Repositories
{
    public class UsersPrivilegesRepository : GenericRepository<UsersPrivileges>, IUsersPrivilegesRepository
    {
        public UsersPrivilegesRepository(ApplicationDbContext applicationDbContext) 
            : base(applicationDbContext)
        {
            
        }
    }
}
