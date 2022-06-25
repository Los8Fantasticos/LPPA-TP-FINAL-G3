using System;
using Transversal.Helpers.JWT;

namespace Api.JwT
{
    public class RefreshTokenSettings : IRefreshTokenSettings
    {
        public TimeSpan duration { get; set; }

    }
}
