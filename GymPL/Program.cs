using GymBLL.Common;
using GymDAL.Entities.Users;
using GymPL.Global;
using GymBLL.Service.Abstract;
using GymBLL.Service.Implementation;
using Hangfire;
using MenoMVC.Languages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using TestMVC.DAL.DB;
using GymPL.Services;
using GymDAL.Common;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using GymBLL.Validation;
using Microsoft.AspNetCore.Localization;
using System.Reflection;
using GymPL.Hubs;
using GymPL.Middleware;
using GymBLL.Service.Abstract.Communication;
using GymBLL.Service.Abstract.Financial;
using GymBLL.Service.Implementation.Financial;
using GymBLL.Service.Implementation.Financial;
using GymBLL.Service.Implementation.Communication;
using GymBLL.Service.Implementation.Communication;
using GymBLL.Service.Abstract.Workout;
using GymBLL.Service.Abstract.Workout;
using GymBLL.Service.Implementation.Workout;
using GymBLL.Service.Abstract.Report;
using GymBLL.Service.Abstract.Report;
using GymBLL.Service.Implementation.Report;
using GymBLL.Service.Abstract.AI;
using GymBLL.Service.Implementation.AI;
using GymBLL.Service.Abstract.Trainer;
using GymBLL.Service.Implementation.Trainer;
using GymBLL.Service.Abstract.Member;
using GymBLL.Service.Implementation.Member;

namespace GymPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
           
            builder.Host.UseSerilog();

            try
            {
                Log.Information("Starting GymMVC Web Application...");

                // Custom Services
                builder.Services.AddSignalR();
                builder.Services.AddScoped<IFileUploadService, FileUploadService>();
                builder.Services.AddScoped<IRazorViewRenderer, RazorViewRenderer>();
                builder.Services.AddScoped<IRealTimeNotificationService, RealTimeNotificationService>();
                builder.Services.AddScoped<IStripeService, StripeService>();
                builder.Services.AddScoped<IChatService, ChatService>();
                builder.Services.AddScoped<ITrainerReviewService, TrainerReviewService>();
                builder.Services.AddScoped<IWorkoutLogService, WorkoutLogService>();
                builder.Services.AddScoped<IReportService, ReportService>();
                builder.Services.AddScoped<IAIService, AIService>();
                builder.Services.AddScoped<IWeightLogService, WeightLogService>();
                builder.Services.AddScoped<IEmailService, EmailService>();
                
                // 2. Configure Controllers and FluentValidation
                builder.Services.AddControllersWithViews()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization(op =>
                    {
                        op.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
                    });

                // Register FluentValidation
                builder.Services.AddFluentValidationAutoValidation();
                builder.Services.AddValidatorsFromAssemblyContaining<LoginUserVMValidator>();

                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<GymDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // 3. Configure Hangfire
                builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
                builder.Services.AddHangfireServer();
                builder.Services.AddScoped<CleanupJob>();

                // Configure Identity
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                }).AddEntityFrameworkStores<GymDbContext>()
                  .AddDefaultTokenProviders();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowSpecificOrigins",
                        policy =>
                        {
                            policy.WithOrigins(
                                    "https://localhost:139",
                                    "http://localhost:139",
                                    "https://localhost:5001",
                                    "http://localhost:5000")
                                   .AllowAnyHeader()
                                   .AllowAnyMethod()
                                   .AllowCredentials();
                        });
                });

                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                });

                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });

                builder.Services.Configure<GymSettings>(builder.Configuration.GetSection("GymSettings"));
                builder.Services.Configure<GymBLL.Common.StripeSettings>(builder.Configuration.GetSection("Stripe"));

                builder.Services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                    });

                builder.Services.AddModularDataAccessLayer();
                builder.Services.AddModularBusinessLogicLayer();
             
                var app = builder.Build();

                // 4. Global Error Handling
                app.UseGlobalExceptionMiddleware();

                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();  // This shows detailed errors
                  
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    app.UseHsts();
                }
                app.UseHttpsRedirection();
                app.UseStaticFiles(); // Fixed: Added UseStaticFiles before Routing
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseCors("AllowSpecificOrigins");

                var supportedLanguages = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG"),
                };

                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture("en-US"),
                    SupportedCultures = supportedLanguages,
                    SupportedUICultures = supportedLanguages,
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

                // 5. Register Recurring Jobs
                using (var scope = app.Services.CreateScope())
                {
                    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                    recurringJobManager.AddOrUpdate<CleanupJob>(
                        "cleanup-expired-registrations",
                        job => job.ExecuteAsync(),
                        Cron.Daily);
                }

                app.MapHub<NotificationHub>("/notificationHub");
                app.MapHub<ChatHub>("/chatHub");

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
