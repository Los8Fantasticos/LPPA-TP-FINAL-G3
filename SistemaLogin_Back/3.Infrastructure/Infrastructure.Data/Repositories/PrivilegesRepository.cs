using Core.Contracts.Repositories;
using Core.Domain.DTO;

using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Repositories
{
    public class PrivilegesRepository : GenericRepository<Privileges>, IPrivilegesRepository
    {
        public PrivilegesRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
