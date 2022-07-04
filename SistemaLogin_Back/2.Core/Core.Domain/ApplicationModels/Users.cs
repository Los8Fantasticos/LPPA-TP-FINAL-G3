using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.ApplicationModels
{
    public class Users : IdentityUser
    {
        public string Salt { get; set; }

        public virtual ICollection<RefreshToken> UserRefreshTokens { get; set; }
    }
}
