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
using GymBLL.Service.Implementation.Communication;
using GymBLL.Service.Abstract.Workout;
using GymBLL.Service.Implementation.Workout;
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
                // AIService is registered as a typed HTTP client (see AddHttpClient below).
                builder.Services.AddScoped<IWeightLogService, WeightLogService>();
                // IEmailService is registered in AddModularBusinessLogicLayer().
                
                // 2. Configure Controllers and FluentValidation
                builder.Services.AddControllersWithViews()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization(op =>
                    {
                        op.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
                    })
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                        options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
                    })
                    .AddMvcOptions(options =>
                    {
                        // Global CSRF protection: every unsafe (POST/PUT/DELETE) request must
                        // carry a valid anti-forgery token. Actions that legitimately cannot
                        // carry one (e.g. the Stripe webhook) are opted out with
                        // [IgnoreAntiforgeryToken].
                        options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
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
                builder.Services.AddScoped<SubscriptionExpiryJob>();

                // Configure Identity (hardened password policy)
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredUniqueChars = 4;
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                }).AddEntityFrameworkStores<GymDbContext>()
                  .AddDefaultTokenProviders();

                // Configure CORS from configuration (no hardcoded origins)
                var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                     ?? Array.Empty<string>();
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowSpecificOrigins",
                        policy =>
                        {
                            if (allowedOrigins.Length > 0)
                                policy.WithOrigins(allowedOrigins)
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

                // Google OAuth (enabled only when credentials are provided via
                // user-secrets / environment variables)
                var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
                var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                if (!string.IsNullOrWhiteSpace(googleClientId) && !googleClientId.StartsWith("YOUR_") &&
                    !string.IsNullOrWhiteSpace(googleClientSecret) && !googleClientSecret.StartsWith("YOUR_"))
                {
                    builder.Services.AddAuthentication()
                        .AddGoogle(options =>
                        {
                            options.ClientId = googleClientId;
                            options.ClientSecret = googleClientSecret;
                        });
                }

                // Strongly-typed options (values come from appsettings / user-secrets / env vars)
                builder.Services.Configure<GymSettings>(builder.Configuration.GetSection("GymSettings"));
                builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
                builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
                builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("GeminiSettings"));

                // AIService as a typed HTTP client instead of a raw scoped HttpClient
                builder.Services.AddHttpClient<IAIService, AIService>();



                builder.Services.AddModularDataAccessLayer();
                builder.Services.AddModularBusinessLogicLayer();
             
                var app = builder.Build();

                // Apply EF Core migrations on startup so deployment does not depend on
                // the hand-managed deploy_db.sql script.
                using (var migrateScope = app.Services.CreateScope())
                {
                    var db = migrateScope.ServiceProvider.GetRequiredService<GymDbContext>();
                    db.Database.Migrate();
                }

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
                    new CultureInfo("en-US")
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

                // Hangfire dashboard restricted to privileged roles
                app.UseHangfireDashboard("/TasksDashboard", new DashboardOptions
                {
                    Authorization = new[] { new HangfireDashboardAuthFilter() }
                });

                // 5. Register Recurring Jobs
                using (var scope = app.Services.CreateScope())
                {
                    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                    recurringJobManager.AddOrUpdate<CleanupJob>(
                        "cleanup-expired-registrations",
                        job => job.ExecuteAsync(),
                        Cron.Daily);

                    recurringJobManager.AddOrUpdate<SubscriptionExpiryJob>(
                        "subscription-expiry-check",
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
