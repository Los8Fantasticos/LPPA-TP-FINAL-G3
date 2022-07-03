using System;
using System.Collections.Generic;
using System.Text;
using Core.Domain.DTO;
using Core.Contracts.Repositories;

namespace Infrastructure.Data.Repositories
{
    public class PrivilegesRepository : GenericRepository<Privileges>, IPrivilegesRepository
    {
        public PrivilegesRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {

        }
    }
}
