using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LizardCode.SalmaSalud.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HealthController> _logger;

        public HealthController(
            IConfiguration configuration,
            ILogger<HealthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Health check de la aplicación con verificación de base de datos
        /// </summary>
        /// <returns>Estado de la aplicación y conectividad con la base de datos</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var checks = new List<object>();

                // Verificar base de datos
                var dbStatus = await CheckDatabase();
                checks.Add(dbStatus);

                // Verificar configuraciones críticas
                var configStatus = CheckConfiguration();
                checks.Add(configStatus);

                var duration = DateTime.UtcNow - startTime;
                
                // Determinar estado general
                var hasUnhealthy = checks.Any(c => c.GetType().GetProperty("status")?.GetValue(c)?.ToString() == "Unhealthy");
                var hasDegraded = checks.Any(c => c.GetType().GetProperty("status")?.GetValue(c)?.ToString() == "Degraded");

                var overallStatus = hasUnhealthy ? "Unhealthy" : (hasDegraded ? "Degraded" : "Healthy");
                var statusCode = hasUnhealthy ? 503 : (hasDegraded ? 207 : 200);

                var response = new
                {
                    status = overallStatus,
                    timestamp = DateTime.UtcNow,
                    application = "LizardCode.SalmaSalud.API",
                    version = GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0",
                    duration = duration.TotalMilliseconds,
                    checks = checks
                };

                return StatusCode(statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    application = "LizardCode.SalmaSalud.API",
                    error = "Health check execution failed",
                    exception = ex.Message
                });
            }
        }

        private async Task<object> CheckDatabase()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return new
                    {
                        name = "database",
                        status = "Unhealthy",
                        error = "Connection string not configured"
                    };
                }

                var startTime = DateTime.UtcNow;
                
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                
                using var command = new SqlCommand("SELECT 1", connection);
                await command.ExecuteScalarAsync();

                var duration = DateTime.UtcNow - startTime;

                return new
                {
                    name = "database",
                    status = "Healthy",
                    duration = duration.TotalMilliseconds,
                    server = connection.DataSource,
                    database = connection.Database
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database health check failed");
                return new
                {
                    name = "database",
                    status = "Unhealthy",
                    error = ex.Message
                };
            }
        }

        private object CheckConfiguration()
        {
            try
            {
                var issues = new List<string>();

                // Verificar configuraciones críticas
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var jwtSecret = _configuration["Jwt:Secret"];
                var apiKey = _configuration["ApiKey"];

                if (string.IsNullOrEmpty(connectionString))
                    issues.Add("Database connection string not configured");
                
                if (string.IsNullOrEmpty(jwtSecret))
                    issues.Add("JWT Secret not configured");
                
                if (string.IsNullOrEmpty(apiKey))
                    issues.Add("API Key not configured");

                var status = issues.Count == 0 ? "Healthy" : (issues.Count <= 1 ? "Degraded" : "Unhealthy");

                return new
                {
                    name = "configuration",
                    status = status,
                    issues = issues.Count > 0 ? issues : null
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    name = "configuration",
                    status = "Unhealthy",
                    error = ex.Message
                };
            }
        }
    }
}