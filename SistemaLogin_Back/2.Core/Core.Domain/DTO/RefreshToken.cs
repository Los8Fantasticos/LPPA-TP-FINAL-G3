using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO
{
    public class RefreshToken
    {
        public RefreshToken() { }

        public RefreshToken(string userId, string token, DateTime expirationDate)
        {
            UserId = userId;
            Token = token;
            Expires = expirationDate;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }

    }
}
