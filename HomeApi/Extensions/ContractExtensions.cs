using System.Globalization;
using HomeApi.Models;
using HomeApi.Models.Response;
using AirQuality = HomeApi.Models.AirQuality;

namespace HomeApi.Extensions;

public static class ContractExtensions
{
    /// <summary>
    /// Converts weather related data into a <see cref="WeatherInformation"/> contract.
    /// </summary>
    /// <param name="weather">The weather data to map.</param>
    /// <param name="locationName">The name of the location.</param>
    /// <param name="auroraForecast">Optional aurora forecast data.</param>
    /// <param name="forecasts">A list of weather forecasts.</param>
    /// <returns>
    /// A <see cref="WeatherInformation"/> object populated with current weather, aurora probability, and forecast information.
    /// </returns>
    public static WeatherInformation ToContract(
        this WeatherData? weather, 
        string? locationName,
        AuroraForecastApiResponse? auroraForecast,
        List<Models.Forecast>? forecasts)
    {
        return new WeatherInformation
        {
            CityName = locationName,
            Current = new Models.Current
            {
                Date = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                Feelslike = weather.Current.Feelslike_C,
                IsDay = weather.Current.Is_Day,
                WindPerMeterSecond = ConvertToMeterPerSecond(weather.Current.Wind_Kph),
                WindGustPerMeterSecond = ConvertToMeterPerSecond(weather.Current.Gust_Kph),
                Temperature = weather.Current.Temp_C,
                LastUpdated = weather.Current.Last_Updated,
                Cloud = weather.Current.Cloud,
                WindDirection = weather.Current.Wind_Dir,
                WeatherDataLocation = new Models.Location
                {
                    Name = weather.Location.Name,
                    Region = weather.Location.Region,
                    Country = weather.Location.Country,
                    Lat = weather.Location.Lat,
                    Lon = weather.Location.Lon
                },
                AirQuality = new AirQuality
                {
                    Co = weather.Current.Air_Quality.Co, // Carbon Monoxide
                    No2 = weather.Current.Air_Quality.No2, // Nitrogen Dioxide
                    O3 = weather.Current.Air_Quality.O3, // Ozone
                    So2 = weather.Current.Air_Quality.So2, // Sulfur Dioxide
                    Pm10 = weather.Current.Air_Quality.Pm10, // Particulate Matter 10 micrometers or less
                    Pm2_5 = weather.Current.Air_Quality.Pm2_5, // Particulate Matter 2.5 micrometers or less
                    Us_Epa_Index = weather.Current.Air_Quality.Us_Epa_Index, // US EPA Air Quality Index
                    Gb_Defra_Index = weather.Current.Air_Quality.Gb_Defra_Index, // UK DEFRA Air Quality Index
                },
                AuroraProbability = new Probability
                {
                    Date = auroraForecast?.Date ?? DateTime.Now,
                    Calculated = new CalculatedProbability
                    {
                        Value = auroraForecast?.Probability.Calculated.Value ?? 0,
                        Colour = auroraForecast?.Probability.Calculated.Colour ?? string.Empty,
                        Lat = auroraForecast?.Probability.Calculated.Lat ?? 0,
                        Long = auroraForecast?.Probability.Calculated.Long ?? 0
                    },
                    Colour = auroraForecast?.Probability.Colour ?? string.Empty,
                    Value = auroraForecast?.Probability.Value.ToString() ?? "No data",
                    HighestProbability = new Highest
                    {
                        Colour = auroraForecast?.Probability.Highest.Colour ?? string.Empty,
                        Lat = auroraForecast?.Probability.Highest.Lat ?? 0,
                        Long = auroraForecast?.Probability.Highest.Long ?? 0,
                        Value = auroraForecast?.Probability.Highest.Value ?? 0,
                        Date = auroraForecast?.Probability.Highest.Date ?? DateTime.Now
                    }
                }
            },
            Forecast = forecasts
        };

        double ConvertToMeterPerSecond(double value = 0)
        {
            if (value > 0)
                return value / 3.6;

            return value;
        }
    }
    
    public static Models.Forecast ToForecast(this ForecastDay day)
    {
        return new Models.Forecast
        {
            Date = day.Date,
            MaxTempC = day.Day.Maxtemp_C,
            MinTempC = day.Day.Mintemp_C,
            DayIcon = day.Day.Condition.Icon,
            IconCode = day.Day.Condition.Code,
            ChanceOfRain = day.Day.Daily_Chance_Of_Rain,
            Astro = new Models.Astro
            {
                Moon_Illumination = day.Astro.Moon_Illumination,
                Moon_Phase = day.Astro.Moon_Phase,
                Moonrise = day.Astro.Moonrise,
                Moonset = day.Astro.Moonset,
                Sunrise = day.Astro.Sunrise,
                Sunset = day.Astro.Sunset
            },
            Day = ToWeatherSummary(day.Hour.Where(h => h.Is_Day == 1).ToList()),
            Night = ToWeatherSummary(day.Hour.Where(h => h.Is_Day == 0).ToList())
        };
    }
    
    private static WeatherSummary? ToWeatherSummary(List<Hour> hours)
    {
        if (hours.Count == 0) 
            return null;

        return new WeatherSummary
        {
            ConditionText = hours.GroupBy(h => h.Condition.Text).OrderByDescending(g => g.Count()).First().Key,
            ConditionIcon = hours.GroupBy(h => h.Condition.Icon).OrderByDescending(g => g.Count()).First().Key,
            AvgTempC = Math.Round(hours.Average(h => h.Temp_C), 1),
            AvgFeelslikeC = Math.Round(hours.Average(h => h.Feelslike_C), 1),
            TotalChanceOfRain = (int)Math.Round(hours.Average(h => h.Chance_Of_Rain)),
            TotalChanceOfSnow = (int)Math.Round(hours.Average(h => h.Chance_Of_Snow))
        };
    }
    
    public static List<TimeTable>? ToContract(this TrafikLabsApiResponse response)
    {
        if (response?.Departure is null)
            return [];

        return response.Departure.Select(dep => new TimeTable
        {
            LineNumber = dep.ProductAtStop?.DisplayNumber ?? dep.ProductAtStop?.Line,
            LineName = dep.ProductAtStop?.Name,
            TransportType = dep.ProductAtStop?.CatOutL,
            Operator = dep.ProductAtStop?.Operator,
            StopName = dep.Stop,
            DepartureTime = $"{dep.Date} {dep.Time}",
            Direction = dep.Direction,
            JourneyDetailRef = dep.JourneyDetailRef?.Ref,
            Notes = dep.Notes?.Note?.Select(n => n.Value).ToList() ?? [],
            InternalTransportationName = dep.ProductAtStop?.InternalName
        }).ToList();
    }
}