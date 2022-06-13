using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO
{
    public class Users : IdentityUser
    {
        public string Salt { get; set; }
    }
}
