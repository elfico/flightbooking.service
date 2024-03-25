using FlightBooking.Service.Data;
using FlightBooking.Service.Data.Configs;
using FlightBooking.Service.Data.Repository;
using FlightBooking.Service.Middleware;
using FlightBooking.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Text;

namespace FlightBooking.Service
{
    public class Startup
    {
        public static readonly LoggerFactory _myLoggerFactory =
            new LoggerFactory(new[]
            {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //we keep using NewtonSoft so that serialization of reference loop can be ignored, especially because of EFCore
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(x => x.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddAutoMapper(typeof(Startup));

            //Configuration for SQL Servr and MySql
            string mysqlConnectionString = Configuration.GetConnectionString("FlightBookingServiceDb_Mysql")!;
            var mySqlServerVersion = new MySqlServerVersion(new Version(8, 0, 36));

            services.AddDbContext<FlightBookingContext>(options =>
            {
                //options.UseLoggerFactory(_myLoggerFactory).EnableSensitiveDataLogging(); //DEV: ENABLE TO SEE SQL Queries

                //To Use Sql Server
                //options.UseSqlServer(Configuration.GetConnectionString("FlightBookingServiceDb"));

                //To Use MySql
                options.UseMySql(mysqlConnectionString, mySqlServerVersion, opt => opt.EnableRetryOnFailure())
                    .LogTo(Console.WriteLine, LogLevel.Warning)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //For OpenId Connect tokens
                options.Authority = Configuration["JWTConfig:Issuer"];
                options.Audience = Configuration["JWTConfig:Issuer"];
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers = [Configuration["JWTConfig:Issuer"], Configuration["JWTConfig:Issuer"]],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTConfig:Key"]!)),
                };
            });

            services.AddCors(option =>
            {
                option.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddSignalR();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            //Add our custom services
            services.AddRepository();
            services.AddServices();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Flight Booking API", Version = "v1" });
            });

            //Add our configs
            services.AddConfigSettings(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0
            //The following Startup.Configure method adds middleware components for common app scenarios:

            //1. Exception / error handling (HTTP Strict Transport Security Protocol in prod)
            //2. HTTPS redirection
            //3. Static File Middleware
            //4. Cookie Policy Middleware
            //5. Routing Middleware (UseRouting) to route requests.
            //5. Cors
            //5. Custom route
            //6. Authentication Middleware
            //7. Authorization Middleware
            //8. Session Middleware
            //9. Endpoint Routing Middleware (UseEndpoints

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseErrorHandlingMiddleware(); //custom error handler
                                                  //app.UseExceptionHandler();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();//this was moved here since the BasicAuthMiddleware below is also authentication and cors must come before authentication.

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseSwagger();
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Booking API");
                });
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}