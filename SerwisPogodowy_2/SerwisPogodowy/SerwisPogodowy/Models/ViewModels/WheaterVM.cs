namespace SerwisPogodowy.Models.ViewModels
{
    public class WheaterVM
    {
        // Dane pogodowe
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public DateTime Date { get; set; }

        // Ścieżka do ikony pogodowej
        public string IconUrl => !string.IsNullOrEmpty(Icon)
            ? $"https://openweathermap.org/img/wn/{Icon}@2x.png"
            : "";
    }
}
