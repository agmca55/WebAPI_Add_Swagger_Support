using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

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

        [Authorize(Roles = "Admin")]
        [HttpGet("for-admins")]
        public List<Product> GetProducts()   
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Admin 1", Price = 19.99m },
                new Product { Id = 2, Name = "Admin 2", Price = 29.99m },
                new Product { Id = 3, Name = "Admin 3", Price = 39.99m }
            };
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("for-users")]
        public List<Product> GetProductForUsers() 
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "User 1", Price = 19.99m },
                new Product { Id = 2, Name = "User 2", Price = 29.99m },
                new Product { Id = 3, Name = "User 3", Price = 39.99m }
            };
        }
    }
}
