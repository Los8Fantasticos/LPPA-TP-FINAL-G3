using Api.Mapping;
using IoC.Resolver;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Api.Configurations;
using Transversal.Extensions;
using Core.Contracts.Data;
using Transversal.Helpers.JWT;
using Api.JwT;
using Core.Domain.ApplicationModels;
using Transversal.Helpers;
using Transversal.EmailService.Configurations;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System.IO;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug());


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>
            (
                options => options
                .UseSqlServer(GetConnectionString(), builder =>
                     builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)) //Al contexto le agrego la conexion de la base de datos

                //En esta parte configuramos el entity framework para ver los querys en consola (IMPORTANTE: desactivarlo en produccion)
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(_loggerFactory)
            );
            
            services.AddIdentity<Users, Privileges>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                //options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            //var r = new EmailSendGridConfiguration();
            //var result = r.GetType().GetFullNameSections();
            
            services.AddConfig<FrontConfiguration>(Configuration, nameof(FrontConfiguration));
            services.AddConfig<ActionLoggerMiddlewareConfiguration>(Configuration, nameof(ActionLoggerMiddlewareConfiguration));
            
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ApiMapping());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            services.AddHttpClient();
            services.ConfigureJwt(Configuration);
            services.ConfigureSwagger();
            services.AddSingleton(mapper); // Singleton al Mapper para los controllers (ahi se haria el traspaso de clases)
            services.ConfigureIoC(Configuration); //LLamo a la clase de IoCRegister que contiene IServiceCollection 
            services.ConfigureLogger(Configuration); //Configuramos el Logger. Vamos a guardarlo en un txt, en la base de datos y enviarlo por mail.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, RoleManager<Privileges> _roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                context.Database.Migrate(); //Cuando se ejecuta la aplicación se ejecuta el metodo update-database de dotnet ef core...
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = String.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "UAI LPPA-FINAL API V1");
            });

            
            app.UseHttpsRedirection();

            var corsAllowAll = Configuration["CorsAllowedAllHosts"] ?? "false";
            app.UseCors(GetCorsConfig(corsAllowAll == "true"));
            string result = context.GetType().Assembly.Location;
            FileInfo file = new FileInfo(result);
            var Infraestructura = file.Directory.Parent.Parent.Parent.Parent.Parent.FullName + @"\3.Infrastructure\Infrastructure.Data\TemplatesEmail\";
            

            AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath);
            AppDomain.CurrentDomain.SetData("WebRootPath", env.WebRootPath);
            AppDomain.CurrentDomain.SetData("InfraestructuraRootPath", Infraestructura);

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private Action<CorsPolicyBuilder> GetCorsConfig(bool allowAnyOrigin)
        {
            void configAllowSpecific(CorsPolicyBuilder configurePolicy)
            {
                string origins = Configuration.GetSection("AllowedOrigins").Value;

                configurePolicy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(origins.Split(","))
                .AllowCredentials();
            }

            void configAllowAll(CorsPolicyBuilder configurePolicy)
            {
                configurePolicy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            }

            if (allowAnyOrigin) return configAllowAll;
            else return configAllowSpecific;
        }


        private string GetConnectionString()
        {
            var connectionString = Configuration.GetConnectionString("SqlConnection");
            return connectionString;
        }
    }
}
