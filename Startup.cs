﻿using AutoMapper;
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

namespace EliteForce
{
    public class Startup
    {

        public IConfiguration _configuration { get; }
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // readonly string AllowedSpecificOrigins = "_myAllowSpecificOrigins";



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            });


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
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false; // Register will not succeed until the user confirms his email
                options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
              .AddRoles<IdentityRole>()
              .AddEntityFrameworkStores<EliteDataContext>()
              .AddDefaultTokenProviders(); // This will configure the token generator for email, phone number and 2 factor authentication that will be sent to the user via mail service. 
                                           // After this step, add the authentication middleware to the application request pipeline in the in the configure method.

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        // Pass in the options to validate the token when it gets sent from the front-end
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuerSigningKey = true,  // Check if our key is valid
            //            ValidAudience = "http://localhost:4200",
            //            ValidIssuer = "http://localhost:5001",
            //            ValidateLifetime = true,
            //            RequireExpirationTime = true,
            //            // This key is required as a byte[] so has to be encrypted
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("Data:AppSettings:Token").Value)),
            //            ValidateIssuer = false, // Issuer in this case is localhost, so we wouldn't validate against that in this method
            //            ValidateAudience = false // Audience in this case is localhost as well
            //        };
            //    });


            services.AddAuthentication( cfg =>
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


            //services.AddAuthentication(auth => {
            //    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //    .AddJwtBearer(options => {
            //        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidAudience = "http://localhost:5001",
            //            ValidIssuer = "http://localhost:5001",
            //            RequireExpirationTime = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is the key that we will use in the encryption, which should be very long, like 1234567898")),
            //            ValidateIssuerSigningKey = true
            //        };
            //    });
            /* Identity plumbing ends */

            services.AddCors(options => {
                options.AddPolicy("ElitePolice", builder => {
                    builder
                    .WithOrigins("http://localhost:4200", "http://localhost:5001")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });


            //services.AddCors(options => {
            //    options.AddPolicy(AllowedSpecificOrigins, builder => {
            //        builder.WithOrigins("http://localhost:4200") // Add other origins in the string, separated with comas.
            //        .AllowAnyHeader()
            //        .AllowAnyMethod()
            //        .AllowCredentials(); // allow credentials;
            //    });
            //});

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
            // Tell what kind of authentication we gonna be using

            var mailKitOptions = _configuration.GetSection("Email").Get<MailKitOptions>();
            services.AddMailKit(config => config.UseMailKit(mailKitOptions));


            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //    .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.Expired)
                //    .Build();
                //config.DefaultPolicy = defaultAuthPolicy;

                //config.AddPolicy("Claim.DoB", policyBuilder => {
                //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                //});

                config.AddPolicy(Policies.Pilot, Policies.PilotPolicy());
                config.AddPolicy(Policies.News, Policies.NewsPolicy());
                config.AddPolicy(Policies.Manager, Policies.ManagerPolicy());
                config.AddPolicy(Policies.SuperAdmin, Policies.SuperAdminPolicy());

                //config.AddPolicy("NewsManager", policy =>
                //{
                //    policy.RequireClaim("NewsManager", "1");
                //});

                //config.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                //.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                //.RequireAuthenticatedUser().Build());
            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
        }


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
            app.UseCors("ElitePolice");

            // setup the identity middleware
            app.UseAuthentication(); // Know who you are // This will decrypt and validate the token, based on the configurations specified in the AddJwtBearer()
            app.UseAuthorization(); // Now that we know who you are, are you allowed to go here or there?



            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "api/{controller}/{action=Index}/{id?}");
            //});

            // Initialize the database with seed data
            

            /* Setup the attribute based routing  */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });


            DbInitializer.Initialize(context);

            //app.UseSpa(spa =>
            //{
            //    // To learn more about options for serving an Angular SPA from ASP.NET Core,
            //    // see https://go.microsoft.com/fwlink/?linkid=864501

            //    spa.Options.SourcePath = "ClientApp";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseAngularCliServer(npmScript: "start");
            //    }
            //});
        }
    }
}





//namespace EliteForce
//{
//    public class Startup
//    {

//        public IConfiguration _configuration { get; }
//        public Startup(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        readonly string AllowedSpecificOrigins = "_myAllowSpecificOrigins";



//        // This method gets called by the runtime. Use this method to add services to the container.
//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddControllersWithViews(setupAction =>
//            {
//                setupAction.ReturnHttpNotAcceptable = true;
//            });

//            // Take care of AutoMapper profiles for mapping of DTOs to Entities and vice versa.
//            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//            // Add the HttpContextAccessor to the dependency container
//            services.AddHttpContextAccessor();

//            services.AddDbContext<EliteDataContext>(options => options.UseSqlServer(_configuration["Data:EliteForce:ConnectionString"]));

//            /* Identity plumbing starts */
//            services.AddDefaultIdentity<User>(options =>
//            {
//                options.SignIn.RequireConfirmedAccount = false;
//                options.Password.RequireDigit = true;
//                options.Password.RequireLowercase = true;
//                options.Password.RequireUppercase = false;
//                options.Password.RequireNonAlphanumeric = false;
//                options.Password.RequiredLength = 6;
//                options.Password.RequiredUniqueChars = 1;

//                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//                options.Lockout.MaxFailedAccessAttempts = 5;
//                options.Lockout.AllowedForNewUsers = true;

//                options.User.RequireUniqueEmail = false;
//                options.SignIn.RequireConfirmedEmail = false; // Register will not succeed until the user confirms his email
//                options.User.AllowedUserNameCharacters =
//        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//            })
//              .AddRoles<IdentityRole>()
//              .AddDefaultTokenProviders()
//              .AddEntityFrameworkStores<EliteDataContext>(); // After this step, add the authentication middleware to the application request pipeline in the in the configure method.

//            services.AddIdentityServer()
//                .AddApiAuthorization<User, EliteDataContext>();

//            services.AddAuthentication()
//                .AddIdentityServerJwt();
//            /* Identity plumbing ends */

//            services.AddCors(options => {
//                options.AddPolicy(AllowedSpecificOrigins, builder => {
//                    builder.WithOrigins("http://localhost:4200") // Add other origins in the string, separated with comas.
//                    .AllowAnyMethod()
//                    .AllowAnyHeader();
//                });
//            });

//            services.Configure<FormOptions>(o =>
//            {
//                o.ValueLengthLimit = int.MaxValue;
//                o.MultipartBodyLengthLimit = int.MaxValue;
//                o.MemoryBufferThreshold = int.MaxValue;
//            });


//            // In production, the Angular files will be served from this directory
//            //services.AddSpaStaticFiles(configuration =>
//            //{
//            //    configuration.RootPath = "ClientApp/dist";
//            //});
//            services.AddHealthChecks();
//            services.AddScoped<IAuthRepository, AuthRepository>();
//            services.AddScoped<IUserRepository, UserRepository>();
//            services.AddScoped<IImageRepository, ImageRepository>();
//            services.AddScoped<IUnitOfWork, UnitOfWork>();
//            services.AddScoped<IConfirmResp, ConfirmResp>();
//            // Tell what kind of authentication we gonna be using

//            var mailKitOptions = _configuration.GetSection("Email").Get<MailKitOptions>();
//            services.AddMailKit(config => config.UseMailKit(mailKitOptions));

//            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                .AddJwtBearer(options => { // Pass in the options to validate the token
//                    options.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuerSigningKey = true,  // Check if our key is valid

//                        // This key is required as a byte[] so has to be encrypted
//                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
//                       .GetBytes(_configuration.GetSection("Data:AppSettings:Token").Value)),
//                        ValidateIssuer = false, // Issuer in this case is localhost, so we wouldn't validate against that in this method
//                        ValidateAudience = false // Audience in this case is localhost as well
//                    };
//                });

//            services.AddAuthorization(config =>
//            {
//                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
//                //var defaultAuthPolicy = defaultAuthBuilder
//                //    .RequireAuthenticatedUser()
//                //    .RequireClaim(ClaimTypes.Expired)
//                //    .Build();
//                //config.DefaultPolicy = defaultAuthPolicy;

//                //config.AddPolicy("Claim.DoB", policyBuilder => {
//                //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
//                //});

//                config.AddPolicy("Claim.DoB", policyBuilder =>
//                {
//                    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
//                });
//            });

//            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
//        }


//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EliteDataContext context)
//        {
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseExceptionHandler(appBuilder => {
//                    appBuilder.Run(async context => {
//                        context.Response.StatusCode = 500;
//                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
//                    });
//                });
//                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//                app.UseHsts();
//            }



//            // app.UseHttpsRedirection();

//            app.UseDefaultFiles(); // It will prompt kestrel to look for default files like index.html from the wwwroot directory ** but this is done prior to deployment.
//            app.UseStaticFiles();
//            app.UseStaticFiles(new StaticFileOptions
//            {
//                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
//                RequestPath = new PathString("/Resources")
//            });

//            //if (!env.IsDevelopment())
//            //{
//            //    app.UseSpaStaticFiles();
//            //}

//            app.UseRouting(); // Routing here helps the app to locate which action method to call first, and because
//            //                  authorization is placed at the action method, the authorization middleware must come after "app.useRouting()" middleware.

//            //app.UseHealthChecks("/hc");
//            app.UseCors(AllowedSpecificOrigins);

//            // setup the identity middleware
//            app.UseAuthentication(); // Know who you are
//            app.UseIdentityServer(); // Store who we know you are, in the identity storage
//            app.UseAuthorization(); // Now that we know who you are, are you allowed to go here or there?

//            //app.UseEndpoints(endpoints =>
//            //{
//            //    endpoints.MapControllerRoute(
//            //        name: "default",
//            //        pattern: "api/{controller}/{action=Index}/{id?}");
//            //});


/* Setup the attribute based routing  */
//app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapControllers();
//                endpoints.MapFallbackToController("Index", "Fallback");
//            });

//            // Initialize the database with seed data
//            DbInitializer.Initialize(context);

//            //app.UseSpa(spa =>
//            //{
//            //    // To learn more about options for serving an Angular SPA from ASP.NET Core,
//            //    // see https://go.microsoft.com/fwlink/?linkid=864501

//            //    spa.Options.SourcePath = "ClientApp";

//            //    if (env.IsDevelopment())
//            //    {
//            //        spa.UseAngularCliServer(npmScript: "start");
//            //    }
//            //});
//        }
//    }
//}
