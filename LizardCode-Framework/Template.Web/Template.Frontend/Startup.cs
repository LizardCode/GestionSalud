using LizardCode.Framework.Aplication.Helpers;
using LizardCode.Framework.Aplication.Startup;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Template.Application;
using Template.Infrastructure;

namespace Template
{
    public class Startup
    {
        private IHttpContextAccessor _httpContextAccessor;

        public IConfiguration Configuration { get; }
        public IHttpContextAccessor HttpContextAccessor => _httpContextAccessor;

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
                .AddNewtonsoftJson();

            services
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor |
                        ForwardedHeaders.XForwardedProto;
                });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Login");
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.AccessDeniedPath = new PathString("/General/Restricted");
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        //ValidAudience = Configuration["JWT:ValidIssuer"],
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                    };
                });

            _httpContextAccessor = new HttpContextAccessor();

            services.AddSingleton(Configuration);
            services.AddSingleton(_httpContextAccessor);

            HttpContextHelper.Configure(_httpContextAccessor);
            Extensions.Configure(Configuration, _httpContextAccessor, null);

            services.AddApplication(Configuration);
            services.AddInfrastructure(Configuration);
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

            AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath);
            AppDomain.CurrentDomain.SetData("WebRootPath", env.WebRootPath);
        }
    }
}
