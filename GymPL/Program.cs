using GymDAL.Entities.Users;
using Hangfire;
using MenoBLL.Commnon;
using MenoDAL.Commnon;
using MenoMVC.Languages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using TestMVC.DAL.DB;

namespace GymPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
                AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/Login");
                }
                );
            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).
               AddRoles<IdentityRole>().AddEntityFrameworkStores<GymDbContext>().
                AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);
            ;

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

            }).AddEntityFrameworkStores<GymDbContext>();
            // Replace this line:
            // 

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
