using APIs.Filters;
using APIs.Jobs;
using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MOJ.Modules.UserManagments.Application.Behaviors;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Application.Common.Models;
using MOJ.Modules.UserManagments.Application.Common.Services;
using MOJ.Modules.UserManagments.Infrastructure.Persistence;
using MOJ.Modules.UserManagments.Infrastructure.Services;
using System.Reflection;
using System.Text;

namespace APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================
            // 1. إضافة الخدمات الأساسية
            // ============================
            ConfigureBasicServices(builder);

            // ============================
            // 2. تكوين قاعدة البيانات
            // ============================
            ConfigureDatabase(builder);

            // ============================
            // 3. تكوين JWT Authentication
            // ============================
            ConfigureJwtAuthentication(builder);

            // ============================
            // 4. تكوين Authorization Policies (✅ هنا قبل Build)
            // ============================
            ConfigureAuthorizationPolicies(builder);

            // ============================
            // 5. تسجيل الخدمات المخصصة
            // ============================
            RegisterCustomServices(builder);

            // ============================
            // 6. تكوين MediatR و FluentValidation
            // ============================
            ConfigureMediatRAndValidation(builder);

            // ============================
            // 7. تكوين Health Checks
            // ============================
            ConfigureHealthChecks(builder);

            // ============================
            // 8. تكوين Hangfire
            // ============================
            ConfigureHangfire(builder);

            var app = builder.Build();

            // ============================
            // 9. تكوين Pipeline
            // ============================
            ConfigurePipeline(app);

            app.Run();
        }

        // =====================================================================
        // 1. الخدمات الأساسية
        // =====================================================================
        private static void ConfigureBasicServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(MOJ.Modules.UserManagments.API.Controllers.UserManagmentController).Assembly);

            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // HttpContext Accessor للوصول إلى HttpContext في الخدمات
            builder.Services.AddHttpContextAccessor();
        }

        // =====================================================================
        // 2. تكوين قاعدة البيانات
        // =====================================================================
        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("APIs");
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });

                options.ConfigureWarnings(warnings =>
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });
        }

        // =====================================================================
        // 3. تكوين JWT Authentication
        // =====================================================================
        private static void ConfigureJwtAuthentication(WebApplicationBuilder builder)
        {
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer is not configured");
            var jwtAudience = builder.Configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience is not configured");

            // Email Settings
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        // =====================================================================
        // 4. تكوين Authorization Policies ( هنا قبل Build)
        // =====================================================================
        private static void ConfigureAuthorizationPolicies(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                // ✅ Policy: المستخدم في قسم IT
                options.AddPolicy("ITDepartment", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "Department" && c.Value == "IT")
                    ));

                // ✅ Policy: Admin في قسم IT
                options.AddPolicy("ITAdmin", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "Department" && c.Value == "IT")  &&
                        context.User.IsInRole("Admin")
                    ));

                // ✅ Policy: أي مستخدم لديه قسم
                options.AddPolicy("DepartmentOnly", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "Department") 
                    ));

                // ✅ Policy: للـ SuperAdmin فقط
                options.AddPolicy("SuperAdminOnly", policy =>
                    policy.RequireRole("SuperAdmin"));

                // ✅ Policy: للمستخدمين النشطين فقط
                options.AddPolicy("ActiveUsers", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "IsActive" && c.Value == "true")
                    ));
            });
        }

        // =====================================================================
        // 5. تسجيل الخدمات المخصصة
        // =====================================================================
        private static void RegisterCustomServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());
            builder.Services.AddTransient<IDateTime, DateTimeService>();
            builder.Services.AddScoped<ITokenService, MOJ.Modules.UserManagments.Infrastructure.Services.TokenService>();
            builder.Services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();
            builder.Services.AddScoped<ICleanupService, CleanupService>();

            // Email Service
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddScoped<IEmailService, EmailServiceDebug>();
            }
            else
            {
                builder.Services.AddScoped<IEmailService, EmailService>();
            }
        }

        // =====================================================================
        // 6. تكوين MediatR و FluentValidation
        // =====================================================================
        private static void ConfigureMediatRAndValidation(WebApplicationBuilder builder)
        {
            var applicationAssembly = Assembly.Load("MOJ.Modules.UserManagments.Application");

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(applicationAssembly);
            });

            builder.Services.AddValidatorsFromAssembly(applicationAssembly);

            // Behaviors
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }

        // =====================================================================
        // 7. تكوين Health Checks
        // =====================================================================
        private static void ConfigureHealthChecks(WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "api" });
        }

        // =====================================================================
        // 8. تكوين Hangfire
        // =====================================================================
        private static void ConfigureHangfire(WebApplicationBuilder builder)
        {
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            builder.Services.AddHangfireServer();
        }

        // =====================================================================
        // 9. تكوين Pipeline
        // =====================================================================
        private static void ConfigurePipeline(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MOJ APIs v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Health Check endpoints
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/database", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains("database")
            });

            // Hangfire Dashboard
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            // Configure Hangfire Jobs
            HangfireJobs.ConfigureRecurringJobs();
            HangfireJobs.EnqueueExampleJobs();
        }
    }
}