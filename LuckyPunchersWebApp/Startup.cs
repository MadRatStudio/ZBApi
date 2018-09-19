using Dal;
using Manager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MRDbIdentity.Domain;
using MRDbIdentity.Infrastructure.Interface;
using MRDbIdentity.Service;
using Microsoft.IdentityModel.Tokens;
using Manager.Options;
using Microsoft.AspNetCore.Http;
using Infrastructure.Entities;
using AutoMapper.Execution;
using AutoMapper;

namespace ZBApi
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mongoClient = new MongoClient(Configuration["ConnectionStrings:Default"]);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,

                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,

                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });


            services.AddIdentityCore<User>()
                .AddDefaultTokenProviders();

            // Identity Services
            services.AddTransient<IUserStore<AppUser>, UserRepository<AppUser>>();
            services.AddTransient<IRoleStore<Role>, RoleRepository>();
            services.AddTransient<IUserRepository<AppUser>, UserRepository<AppUser>>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<SignInManager<User>>();
            services.AddTransient(x => AppUserManager.Create(new MongoClient(Configuration["ConnectionStrings:Default"]).GetDatabase(Configuration["Database:Name"])));
            services.AddTransient(x => new MongoClient(Configuration["ConnectionStrings:Default"]).GetDatabase(Configuration["Database:Name"]));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // managers
            services.AddTransient<AccountManager>();
            services.AddTransient<UserManager>();

            services.AddAutoMapper();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
