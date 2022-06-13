﻿using Core.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Services
{
    public interface IUsersService:IGenericService<Users>
    {
        public Task<bool> CreateUserAsync(Users users, string password);
    }
}
