using HomeApi.Extensions;
using HomeApi.Integration;
using HomeApi.Models;
using HomeApi.Models.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace HomeApi.Handlers;

public static class Weather
{
    public record Command : IRequest<WeatherInformation>;

    public class Handler : IRequestHandler<Command, WeatherInformation>
    {
        private readonly ApiConfiguration _apiConfiguration;
        private readonly ILogger<Handler> _logger;
        private readonly IGeocodingService _geocodingService;
        private readonly IAuroraService _auroraService;
        private readonly IWeatherService _weatherService;

        public Handler(
            IOptions<ApiConfiguration> apiConfiguration,
            ILogger<Handler> logger,
            IGeocodingService geocodingService,
            IAuroraService auroraService,
            IWeatherService weatherService)
        {
            _apiConfiguration = apiConfiguration.Value;
            _logger = logger;
            _geocodingService = geocodingService;
            _auroraService = auroraService;
            _weatherService = weatherService;
        }

        public async Task<WeatherInformation> Handle(Command request, CancellationToken cancellationToken)
        {
            var coordinates = await _geocodingService.TryCallAsync(service =>
                service.GetCoordinatesAsync(_apiConfiguration.DefaultCity),
                _logger, 
                "Failed to get coordinates"
            );
            
            var aurora = await _auroraService.TryCallAsync(service => 
                service.GetAuroraForecastAsync(coordinates?.Lat ?? "0.00", coordinates?.Lon ?? "0.00"), 
                _logger, 
                "Failed to get aurora forecast"
            );
            
            var weather = await _weatherService.TryCallAsync(service => 
                service.GetWeatherAsync(coordinates?.Lat ?? string.Empty, coordinates?.Lon ?? string.Empty), 
                _logger, 
                "Failed to get weather data"
            );

            var forecasts = weather?.Forecast.Forecastday.Select(day => day.ToForecast()).ToList();

            return weather?.ToContract(coordinates?.Name, aurora, forecasts) ?? new WeatherInformation();
        }
    }
}