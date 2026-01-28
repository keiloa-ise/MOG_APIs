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
            builder.Services.AddEndpointsApiExplorer();  // Add Swagger
            builder.Services.AddSwaggerGen();            // Add Swagger

            // Register Services
            //builder.Services.AddSingleton<IRedisService, RedisService>();
            //builder.Services.AddSingleton<ITokenService, TokenService>();
            //builder.Services.AddHostedService<DequeueBackgroundService>();

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
        
