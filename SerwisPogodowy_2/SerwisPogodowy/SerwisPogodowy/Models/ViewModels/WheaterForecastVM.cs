using System.Text;

namespace SerwisPogodowy.Models.ViewModels
{
    public class WheaterForecastVM
    {
        public List<WheaterVM> Forecast { get; set; } = new List<WheaterVM>();

        public string Labels
        {
            get
            {
                List<string> times = new List<string>();
                foreach (var item in Forecast)
                {
                    string dayWithTimeInPolish = item.Date.ToString("dddd HH:mm", new System.Globalization.CultureInfo("pl-PL"));
                    times.Add(dayWithTimeInPolish);
                }
                // Generowanie tablicy JSON z pojedynczymi cudzysłowami
                return Newtonsoft.Json.JsonConvert.SerializeObject(times);
            }
        }

        public string Temperatures
        {
            get
            {
                List<double> uniqueTemperatures = new List<double>();
                foreach (var item in Forecast)
                {
                   uniqueTemperatures.Add(item.Temperature);
                }
                // Generowanie tablicy JSON z pojedynczymi cudzysłowami
                return Newtonsoft.Json.JsonConvert.SerializeObject(uniqueTemperatures);
            }
        }
    }
}
