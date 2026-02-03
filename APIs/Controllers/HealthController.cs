using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();

            var response = new
            {
                status = healthReport.Status.ToString(),
                totalDuration = healthReport.TotalDuration,
                timestamp = DateTime.UtcNow,
                version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                entries = healthReport.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = e.Value.Duration.TotalMilliseconds,
                    description = e.Value.Description,
                    exception = e.Value.Exception?.Message,
                    data = e.Value.Data
                })
            };

            return healthReport.Status == HealthStatus.Healthy
                ? Ok(response)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                message = "API is running",
                timestamp = DateTime.UtcNow,
                version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0"
            });
        }
    }
}
