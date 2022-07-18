using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Domain.ApplicationModels
{
    public class UsersPrivileges : IdentityUserRole<string>
    {
        public virtual ICollection<Users> PrivilegesUsers { get; set; }
    }
}
