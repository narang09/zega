using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZegaFinancials.Services;
using ZegaFinancials.Business;
using ZegaFinancials.Nhibernate.Dao;
using ZegaFinancials.Business.Interfaces;
using System.IO;
using Microsoft.Extensions.Logging;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace ZegaFinancials
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
            var connectionString = Configuration.GetConnectionString("Default");
            var secretKey = Configuration["SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("Jwt Secret Key Not Found.");
            var secretKeyByte = Encoding.ASCII.GetBytes(secretKey); 
            services.AddAuthentication(au => 
            {
                au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt => 
            {
                jwt.RequireHttpsMetadata = true;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyByte),
                    ValidateIssuer = false, 
                    ValidateAudience = false
                };
            });
            services.AddNhibernate(connectionString);
            services.AddFluentValidation();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache 
            services.AddSession(); // for adding the user session
            services.RegisterServicesDI();
            services.RegisterLogicDI();
            services.RegisterDaoDI();
                      
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            // Default JsonSerializerOptions will be camelCase (DotNetCore)
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IInitialDataLogic initialDataLogic, ILoggerFactory loggerFactory)
        {
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
          
            initialDataLogic.CreateDefaultGridHeaders();
            initialDataLogic.CreateDefaultUser();
            initialDataLogic.DeleteLoginActivity();
            initialDataLogic.CreateBlendStrategy();


            //To DO: Need to find other solution for Securing the cookie
            //app.UseCookiePolicy( 
            //    new CookiePolicyOptions
            //    {
            //        Secure = CookieSecurePolicy.Always
            //    });

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
            app.UseHttpsRedirection();
        }
    }
}
