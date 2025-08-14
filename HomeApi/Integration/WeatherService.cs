using HomeApi.Integration.Client.WeatherClient;
using HomeApi.Models.Configuration;
using HomeApi.Models.Response;
using Microsoft.Extensions.Options;

namespace HomeApi.Integration;

public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(string lat, string lon);
}

public class WeatherService(IWeatherClient weatherApi, IOptions<ApiConfiguration> options) : IWeatherService
{
    private readonly ApiConfiguration _apiConfig = options.Value;

    public Task<WeatherData> GetWeatherAsync(string? lat, string? lon)
    {
        var location = $"{lat},{lon}";
        
        if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lon))
        {
            location = _apiConfig.DefaultCity;
        }
        
        return weatherApi.GetForecastAsync(_apiConfig.Keys.Weather, location);
    }
}