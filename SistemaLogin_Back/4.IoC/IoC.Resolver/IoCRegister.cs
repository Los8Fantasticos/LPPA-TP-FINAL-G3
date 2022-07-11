using Core.Business.Services;
using Core.Contracts.Services;
using IoC.Resolver.Register;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Resolver
{
    public static class IoCRegister
    {
        public static IServiceCollection ConfigureIoC(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDataLayer(configuration);
            services.RegisterUnitOfWork();
            services.RegisterBusinessLayer();
            services.AddTransient<IEmailService, EmailService>();
            return services;
        }
    }
}
