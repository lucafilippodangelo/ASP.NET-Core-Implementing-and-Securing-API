using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MyCodeCamp2.Data;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp2.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Versioning;
using MyCodeCamp2.Controllers;

namespace MyCodeCamp2
{
    public class Startup
    {
        //IConfigurationRoot _config;
        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            _env = env;
        }

        public IConfigurationRoot Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            //LD we need to make available the "ConfigurationBuilder" service for any future use.
            services.AddSingleton(Configuration);

            //LD STEP33 now we add the service. Usually for EntityFramework 
            //request we use "AddScoped"
            services.AddScoped<ICampRepository, CampRepository>();

            //LD STEP44 the repository above need to have INJECTED the DBCONTEXT "CampContext"
            services.AddDbContext<CampContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LdConnectionStringMyCodeCamp2")));


            //LD STEP55 we add the dependency to a class in order to SEED the database
            services.AddTransient<CampDbInitializer>();

            //LD STEP27
            services.AddTransient<CampIdentityInitializer>();

            ////LD STEP888
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper();

            //LD STEP51
            services.AddMemoryCache();

            //LD STEPdist1
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "ThisWebsite";
            });

            //LD STEP19
            services.AddIdentity<CampUser, IdentityRole>().AddEntityFrameworkStores<CampContext>();

            //LD STEP21
            services.Configure<IdentityOptions>(config =>
            {
                config.Cookies.ApplicationCookie.Events = //LD STEP22
                  new CookieAuthenticationEvents()
                  {
                      OnRedirectToLogin = (ctx) =>
                    {
                        //LD if the call is from an API and we are going to redirect to the login page, just return error 401 
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200) 
                        {
                            ctx.Response.StatusCode = 401;
                        }

                        return Task.CompletedTask;
                    },
                      OnRedirectToAccessDenied = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 403;
                        }

                        return Task.CompletedTask;
                    }
                  };
            });

            //LD STEP38
            services.AddApiVersioning(cfg =>
            {
                cfg.DefaultApiVersion = new ApiVersion(1, 1);
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.ReportApiVersions = true; //LD this will send back in the header the supported api versions
                //LD STEP44
                var rdr = new HeaderApiVersionReader("ver");
                //rdr.HeaderNames.Add("X-MyCodeCamp-Version");
                //cfg.ApiVersionReader = rdr;

                //LD STEP45
                //cfg.Conventions.Controller<TalksController>()
                //  .HasApiVersion(new ApiVersion(1, 0))
                //  .HasApiVersion(new ApiVersion(1, 1))
                //  .HasApiVersion(new ApiVersion(2, 0))
                //  .Action(m => m.Post(default(string), default(int), default(TalkModel)))
                //    .MapToApiVersion(new ApiVersion(1, 1));
            });


            //LD STEP13
            //services.AddCors();

            //LD STEP15
            services.AddCors(cfg =>
            {
                cfg.AddPolicy("Wildermuth", bldr =>
                {
                    bldr.AllowAnyHeader()
                      .AllowAnyMethod()
                      .WithOrigins("http://wildermuth.com");
                });

                cfg.AddPolicy("AnyGET", bldr =>
                {
                    bldr.AllowAnyHeader()
                      .WithMethods("GET")
                      .AllowAnyOrigin();
                });
            });

            //LD STEP36
            //LD adding the policy that can be called from any action controller
            services.AddAuthorization(cfg =>
            {
                cfg.AddPolicy("SuperUsers", p => p.RequireClaim("SuperUser", "True"));
            });


            // Add framework services.
            services.AddMvc(opt =>
            {
                if (!_env.IsProduction())
                {
                    opt.SslPort = 44342; //LD same port we see in configuration
                }
                //LD STEP12
                opt.Filters.Add(new RequireHttpsAttribute());
            })
              .AddJsonOptions(opt => //LD STEP88 setting to avoid ciclic references of clesses when we use json
              {
                  opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
              });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            CampDbInitializer seeder, //LD STEP66 we add the parameter we want use in "Configure" method and the dependency injection layer will fulfill if possible
            CampIdentityInitializer identitySeeder //LD STEP28
            )
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //LD STEP14
            //app.UseCors(cfg =>
            //{
            //    cfg.AllowAnyHeader()
            //       .AllowAnyMethod()
            //       .WithOrigins("http://wildermuth.com");
            //});

            //LD STEP35
            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                //LD we ask to the framework that if find the token then authenticate authomatically
                AutomaticAuthenticate = true, 
                AutomaticChallenge = true,
                //LD following we have the parameter that we want Bearer will use to validate the token
                TokenValidationParameters = new TokenValidationParameters() 
                {
                    ValidIssuer = Configuration["Tokens:Issuer"],
                    ValidAudience = Configuration["Tokens:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                    ValidateLifetime = true //LD verify that the token is not expired
                }
            });

            //LD STEP19
            app.UseIdentity(); //LD we want tha "Identity" is executed before MVC, we want protect before all the MVC process

            app.UseMvc();

            //seeder.Seed().Wait(); //LD STEP77 here we just call it.
            //identitySeeder.Seed().Wait(); //LD STEP28
        }
    }
}
