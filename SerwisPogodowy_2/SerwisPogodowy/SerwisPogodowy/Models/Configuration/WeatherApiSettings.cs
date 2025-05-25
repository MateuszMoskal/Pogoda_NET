namespace SerwisPogodowy.Models.Configuration
{
    public class WeatherApiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.openweathermap.org";
        public int CacheTimeoutMinutes { get; set; } = 180;
    }
}