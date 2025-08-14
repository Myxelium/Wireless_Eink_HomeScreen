#nullable disable

using HomeApi.Models.Response;

namespace HomeApi.Models;

public class WeatherInformation
{
    public string CityName { get; set; } = "Not defined";
    public Current Current { get; set; } = new();
    public List<Forecast> Forecast { get; set; }
}

public class Current
{
    public string Date { get; set; } = string.Empty;
    public double Feelslike { get; set; } = 0;
    public int IsDay { get; set; } = 1;
    public double WindPerMeterSecond { get; set; } = 0;
    public double WindGustPerMeterSecond { get; set; } = 0;
    public double Temperature { get; set; } = 0;
    public string LastUpdated { get; set; } = string.Empty;
    public int Cloud { get; set; } = 0;
    public string WindDirection { get; set; } = string.Empty;
    public Location WeatherDataLocation { get; set; } = new();
    public AirQuality AirQuality { get; set; }

    public Probability AuroraProbability { get; set; } = new();
}

public class Location
{
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Lat { get; set; } = 0;
    public double Lon { get; set; } = 0;
}

public class Probability
{
    public DateTime Date { get; set; } = DateTime.Now;
    public CalculatedProbability Calculated { get; set; } = new();
    public string Colour { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Highest HighestProbability { get; set; } = new();
}

public class Highest
{
    public DateTime Date { get; set; } = DateTime.Now;
    public string Colour { get; set; } = string.Empty;
    public double Lat { get; set; } = 0;
    public double Long { get; set; } = 0;
    public int Value { get; set; } = 0;
}

public class Forecast
{
    public string Date { get; set; } = string.Empty;
    public double MinTempC { get; set; } = 0;
    public double MaxTempC { get; set; } = 0;
    public string DayIcon { get; set; } = string.Empty;
    public WeatherSummary? Day { get; set; } = null;
    public WeatherSummary? Night { get; set; } = null;
    public Astro Astro { get; set; } = new();
    public int IconCode { get; set; } = 0;
    
    public int ChanceOfRain { get; set; }  = 0;
}
public class Astro
{
    public string Sunrise { get; set; } = string.Empty;
    public string Sunset { get; set; } = string.Empty;
    public string Moonrise { get; set; } = string.Empty;
    public string Moonset { get; set; } = string.Empty; 
    public string Moon_Phase { get; set; } = string.Empty;
    public double? Moon_Illumination { get; set; }
}

public class AirQuality
{
    public double Co { get; set; } = 0;
    public double No2 { get; set; } = 0;
    public double O3 { get; set; } = 0;
    public double So2 { get; set; } = 0;
    public double Pm2_5 { get; set; } = 0;
    public double Pm10 { get; set; } = 0;
    public int Us_Epa_Index { get; set; } = 0;
    public int Gb_Defra_Index { get; set; } = 0;
}

public class WeatherSummary
{
    public string ConditionText { get; set; } = string.Empty;
    public string ConditionIcon { get; set; } = string.Empty;
    public double AvgTempC { get; set; } = 0;
    public double AvgFeelslikeC { get; set; } = 0;
    public int TotalChanceOfRain { get; set; } = 0;
    public int TotalChanceOfSnow { get; set; } = 0;
}