﻿using Core.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IPrivilegesService : IGenericService<Privileges>
    {
        public Task<bool> CreatePrivilegeAsync(Privileges privileges);
    }
}
