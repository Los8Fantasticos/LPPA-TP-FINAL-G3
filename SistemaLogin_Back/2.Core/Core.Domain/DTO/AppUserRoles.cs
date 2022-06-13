using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Domain.DTO
{
    public class AppUserRoles : IdentityUserRole<string>
    {
        public int Id { get; set; }
    }
}
