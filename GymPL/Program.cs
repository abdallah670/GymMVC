using GymBLL.Commnon;
using GymDAL.Entities.Users;
using GymPL.Global;
using GymPL.Services;
using Hangfire;

using MenoDAL.Commnon;
using MenoMVC.Languages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Runtime;
using TestMVC.DAL.DB;

namespace GymPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<IFileUploadService, FileUploadService>();
            // Add services to the container.
            builder.Services.AddControllersWithViews().
                AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).
                AddDataAnnotationsLocalization(op =>
                {
                    op.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
                });
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<GymDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            builder.Services.AddHangfireServer();
            
            // First add Identity (it sets up its own authentication internally)
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedAccount = false; // Changed to false for easier testing
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            }).AddEntityFrameworkStores<GymDbContext>()
              .AddDefaultTokenProviders();
            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins(
                                "https://localhost:139",  // Your frontend port
                                "http://localhost:139",    // HTTP version
                                "https://localhost:5001",  // Default development
                                "http://localhost:5000")   // Alternative
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

           
          
            // Then configure authentication to add our CustomAuth cookie scheme
            // and set it as the default (this must come AFTER AddIdentity)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });
            
            // Add the CustomAuth cookie scheme
            builder.Services.AddAuthentication()
                .AddCookie("CustomAuth", options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                });
            // Replace this line:
            // 
            builder.Services.Configure<GymSettings>(builder.Configuration.GetSection("GymSettings"));
            // With this line:
            builder.Services.AddModularDataAccessLayer();
            builder.Services.AddModularBusinessLogicLayer();
         

            var app = builder.Build();
            var SupportedLanguages = new[]
            {
             new CultureInfo("en-US"),
              new CultureInfo("ar-EG"),
           };

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowSpecificOrigins");

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = SupportedLanguages,
                SupportedUICultures = SupportedLanguages,
                FallBackToParentCultures = true,
                FallBackToParentUICultures = true,
                RequestCultureProviders = new List<IRequestCultureProvider>
            {
               new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),
               new AcceptLanguageHeaderRequestCultureProvider()
             }
            });
            app.UseHangfireDashboard("/TasksDashboard");


            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
