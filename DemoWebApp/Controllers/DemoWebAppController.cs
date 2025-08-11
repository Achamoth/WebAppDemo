using DemoWebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DemoWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoWebAppController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DemoWebAppController> _logger;
        private readonly ConnectionStrings _connectionStrings;

        public DemoWebAppController(
            ILogger<DemoWebAppController> logger,
            IOptions<ConnectionStrings> connectionStrings)
        {
            _logger = logger;
            _connectionStrings = connectionStrings.Value;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("CreateDbEntry")]
        public async Task CreateDbEntry()
        {
            var dbConnectionString = _connectionStrings.DatabaseConnectionString;
            await using var connection = new SqlConnection(dbConnectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = @"insert into dbo.TestTable(Id, TestValue)
values (newid(), 'Test')";
            await command.ExecuteNonQueryAsync();
        }
    }
}
