using Microsoft.AspNetCore.Mvc;
using System.Data;
using Newtonsoft.Json;

namespace WebAPI.A6.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        
        [HttpGet]
        public string TestGet(int id)
        {
            System.Threading.Thread.Sleep(1000 * 10);
            return id + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
        [HttpGet]
        public DataTable GetTable()
        {
            DataTable dt = new DataTable();
            DataColumn dataColumn = new DataColumn("id");
            DataColumn dataColumn1 = new DataColumn("name");
            dt.Columns.Add(dataColumn);
            dt.Columns.Add(dataColumn1);
            DataRow dataRow = dt.NewRow();
            dataRow["id"] = "1";
            dataRow["name"] = "zzd";
            dt.Rows.Add(dataRow);
            DataRow dataRow1 = dt.NewRow();
            dataRow1["id"] = "2";
            dataRow1["name"] = "sdf";
            dt.Rows.Add(dataRow1);
            return dt;
        }

    }
}