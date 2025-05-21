using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LizardCode.SalmaSalud.Application;
using LizardCode.SalmaSalud.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Startup;
using LizardCode.SalmaSalud.Infrastructure;
using System;
using System.Text;

namespace LizardCode.SalmaSalud
{
    public class Startup
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private IHttpContextAccessor _httpContextAccessor;
        private IMemoryCache _memoryCache;

        public IConfiguration Configuration { get; }
        public IDataProtectionProvider DataProtectionProvider => _dataProtectionProvider;
        public IHttpContextAccessor HttpContextAccessor => _httpContextAccessor;
        public IMemoryCache MemoryCache => _memoryCache;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSession();
            services.AddDataProtection();
            services
                .AddMvc(options =>
                {
                    options.RegisterFilters();
                    options.RegisterBinders();
                })
                .AddSessionStateTempDataProvider()
                .AddNewtonsoftJson();

            services
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor |
                        ForwardedHeaders.XForwardedProto;
                });

            services
                .Configure<RequestLocalizationOptions>(options =>
                {
                    options.SetDefaultCulture("es-AR");
                    options.AddSupportedUICultures("en-US");
                    options.FallBackToParentUICultures = true;
                    options.RequestCultureProviders.Clear();
                });

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Login");
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.AccessDeniedPath = new PathString("/General/Restricted");
                });

            _httpContextAccessor = new HttpContextAccessor();

            services.AddSingleton(Configuration);
            services.AddSingleton(_httpContextAccessor);

            HttpContextHelper.Configure(_httpContextAccessor);
            Extensions.Configure(Configuration, _httpContextAccessor, null);

            services.AddApplication(Configuration);
            services.AddInfrastructure();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Errors");
                app.UseForwardedHeaders();
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Errors/Handle/{0}");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSalmaSaludUtilities();

            AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath);
            AppDomain.CurrentDomain.SetData("WebRootPath", env.WebRootPath);
        }
    }
}
