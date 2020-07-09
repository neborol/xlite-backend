using AutoMapper;
using EliteForce.Data;
using EliteForce.Seeds;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Text;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Identity;
using EliteForce.Entities;
using Microsoft.AspNetCore.Authentication;
using EliteForce.AppWideHelpers;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EliteForce.AuthorizationRequirements;
using EliteForce.Services;
using System.Runtime.InteropServices.ComTypes;
using NLog;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EliteForce.NLoging;

namespace EliteForce
{
    public class Startup
    {
        readonly string MyAllowedOrigins = "EliteCorsPolicy";
        // private readonly LogFactory _logger;
        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "NLog.config"));
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            });

            services.AddLogging();

            // Take care of AutoMapper profiles for mapping of DTOs to Entities and vice versa.
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add the HttpContextAccessor to the dependency container
            services.AddHttpContextAccessor();

            // Configure EntityFramework with the SQL server
            services.AddDbContext<EliteDataContext>(options => options.UseSqlServer(_configuration["Data:EliteForce:ConnectionString"]));

            /* Identity plumbing starts */
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false; // Register will not succeed until the user confirms his email
                options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@$*&#+";
            })
              .AddRoles<IdentityRole>()
              .AddEntityFrameworkStores<EliteDataContext>()
              .AddDefaultTokenProviders(); // This will configure the token generator for email, phone number and 2 factor authentication that will be sent to the user via mail service. 
                                           // After this step, add the authentication middleware to the application request pipeline in the in the configure method.

            services.AddAuthentication(cfg =>
               {
                   cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })
                .AddJwtBearer(cfg => {
                    cfg.RequireHttpsMetadata = false;
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidIssuer = _configuration["Security:Tokens:Issuer"],
                        ValidateAudience = false,
                        ValidAudience = _configuration["Security:Tokens:Audience"],
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:Tokens:Key"])),

                    };
                });


            services.AddCors(options => {
                options.AddPolicy(name: MyAllowedOrigins, builder => {
                    builder
                    .WithOrigins(
                        _configuration["Deploy:Origins:Client1"],
                        _configuration["Deploy:Origins:Client2"],
                        _configuration["Deploy:Origins:Client3"]
                    )
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST", "PUT", "DELETE", "get", "post", "put", "delete")
                    .AllowCredentials();
                });
            });

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });


            // In production, the Angular files will be served from this directory
            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "ClientApp/dist";
            //});
            services.AddHealthChecks();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IConfirmResp, ConfirmResp>();
            services.AddTransient<IMailService, SendGridMailService>();
            services.AddScoped<IFaqRepository, FaqRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IVideosRepository, VideosRepository>();
            services.AddScoped<IMissionPhotosRepository, MissionPhotosRepository>();
            services.AddScoped<IContributionsRepository, ContributionsRepository>();
            services.AddScoped<IFinancesRepository, FinancesRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();
            services.AddScoped<ILoggerManager, LoggerManager>();


            var mailKitOptions = _configuration.GetSection("Email").Get<MailKitOptions>();
            services.AddMailKit(config => config.UseMailKit(mailKitOptions));


            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Pilot, Policies.PilotPolicy());
                config.AddPolicy(Policies.News, Policies.NewsPolicy());
                config.AddPolicy(Policies.Manager, Policies.ManagerPolicy());
                config.AddPolicy(Policies.SuperAdmin, Policies.SuperAdminPolicy());

            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
        }

        //public static void ConfigureLoggerService(this IServiceCollection services) =>
        //    services.AddScoped<ILoggerManager, LoggerManager>();


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EliteDataContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder => {
                    appBuilder.Run(async context => {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }



            // app.UseHttpsRedirection();

            //           app.UseDefaultFiles(); // It will prompt kestrel to look for default files like index.html from the wwwroot directory ** but this is done prior to deployment.
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
                RequestPath = new PathString("/Resources")
            });

            //if (!env.IsDevelopment())
            //{
            //    app.UseSpaStaticFiles();
            //}

            app.UseRouting(); // Routing here helps the app to locate which action method to call first, and because
            //                  authorization is placed at the action method, the authorization middleware must come after "app.useRouting()" middleware.

            //app.UseHealthChecks("/hc");
            app.UseCors("EliteCorsPolicy");

            // setup the identity middleware
            app.UseAuthentication(); // Know who you are // This will decrypt and validate the token, based on the configurations specified in the AddJwtBearer()
            app.UseAuthorization(); // Now that we know who you are, are you allowed to go here or there?


            /* Setup the attribute based routing  */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "api/{controller}/{action=Index}/{id?}");
            //});


            DbInitializer.Initialize(context);

        }
    }
}
