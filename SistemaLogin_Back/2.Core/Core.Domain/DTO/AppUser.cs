using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO
{
    public class AppUser : IdentityUser
    {
        public string Salt { get; set; }
    }
}
