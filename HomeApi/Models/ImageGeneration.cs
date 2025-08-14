namespace HomeApi.Models;

public class Image
{
    public WeatherInformation? Weather { get; set; }
    public List<TimeTable>? TimeTable { get; set; }
}