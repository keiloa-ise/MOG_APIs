using APIs.Filters;
using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using MailKit.Search;
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

            // Add services to the container.
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(MOJ.Modules.UserManagments.API.Controllers.UserManagmentController).Assembly);

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // ≈÷«›… JWT Authentication ›Ì Swagger
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

            //  ﬂÊÌ‰ ﬁ«⁄œ… «·»Ì«‰«  „⁄  Ã«Â· «· Õ–Ì—« 
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

                //  Ã«Â·  Õ–Ì— «· €ÌÌ—«  ›Ì «·‰„Ê–Ã
                options.ConfigureWarnings(warnings =>
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            //  ﬂÊÌ‰ JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is not configured");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer is not configured");
            var jwtAudience = builder.Configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience is not configured");
            
            // Add Email Settings
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
                            // «” Œœ„ Append »œ·« „‰ Add
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Register Services
            builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            builder.Services.AddTransient<IDateTime, DateTimeService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();
            builder.Services.AddScoped<ICleanupService, CleanupService>();
            if (builder.Environment.IsDevelopment())
            {
                // «” Œœ«„ Œœ„… «· ’ÕÌÕ ›Ì »Ì∆… «· ÿÊÌ—
                builder.Services.AddScoped<IEmailService, EmailServiceDebug>();
            }
            else
            {
                // «” Œœ«„ «·Œœ„… «·ÕﬁÌﬁÌ… ›Ì «·≈‰ «Ã
                builder.Services.AddScoped<IEmailService, EmailService>();
            }

            //  ﬂÊÌ‰ MediatR (Mediator Design Pattern)
            var applicationAssembly = Assembly.Load("MOJ.Modules.UserManagments.Application");
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(applicationAssembly);
            });

            // Register FluentValidation
            builder.Services.AddValidatorsFromAssembly(applicationAssembly);

            // Register Behaviors
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // Add Health Checks
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "api" });

            // Add Hangfire
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero, // For preview, it's best to set a short value like Zero, but use TimeSpan.FromSeconds(15) for production to reduce database compression.
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Added Hangfire server for task processing
            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                // Add Swagger middleware
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MOJ APIs v1");
                    options.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
                                                     // options.RoutePrefix = string.Empty; // Set Swagger UI at root
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

            // add Hangfire Dashboard
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                // Õ„«Ì… »”Ìÿ… ··„⁄«Ì‰… - ÌÃ»  €ÌÌ—Â« ›Ì «·≈‰ «Ã
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            // 1. „Â„… ›Ê—Ì… (Fire-and-Forget)
            BackgroundJob.Enqueue(() => Console.WriteLine("Hangfire/Fire-and-Forget job"))
                ;
            // 2. „Â„… „ƒÃ·… (Delayed)
            BackgroundJob.Schedule(
                () => Console.WriteLine("Hangfire/Delayed job"),
                TimeSpan.FromDays(7));

            // 3. „Â„… „ ﬂ——… (Recurring)
            RecurringJob.AddOrUpdate<ICleanupService>(
                "clean-expired-sessions",
                service => service.CleanExpiredSessions(),
                Cron.Daily(2)); // ﬂ· ÌÊ„ ›Ì «·”«⁄… 2 ’»«Õ«

            RecurringJob.AddOrUpdate<ICleanupService>(
                "clean-old-password-histories",
                service => service.CleanOldPasswordHistories(5),
                Cron.Weekly(DayOfWeek.Sunday, 3)); // ﬂ· ÌÊ„ √Õœ ›Ì «·”«⁄… 3 ’»«Õ«

            RecurringJob.AddOrUpdate<ICleanupService>(
                "clean-old-audit-logs",
                service => service.CleanOldAuditLogs(30),
                Cron.Monthly()); // ﬂ· ‘Â—

            // 4. „Â„… „ ”·”·… (Continuations)
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Hangfire/Continuations ValidateOrder"));
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Hangfire/Continuations ProcessPayment"));
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Hangfire/Continuations SendConfirmation"));

            app.Run();
        }
    }
}

