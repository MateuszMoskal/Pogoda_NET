using Microsoft.EntityFrameworkCore;
using SerwisPogodowy.DataBase;
using SerwisPogodowy.Models;

namespace SerwisPogodowy.Repositories
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private readonly DataBaseContext context;
        private readonly ILogger<DataBaseRepository> logger;

        public DataBaseRepository(DataBaseContext context, ILogger<DataBaseRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public User? ReadUser(string email, string hashPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(hashPassword))
            {
                logger.LogWarning("Próba odczytu użytkownika z pustymi parametrami");
                return null;
            }

            try
            {
                return context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashPassword);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas odczytu użytkownika: {Email}", email);
                return null;
            }
        }

        public bool UserExiest(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                logger.LogWarning("Sprawdzanie istnienia użytkownika z pustym emailem");
                return false;
            }

            try
            {
                return context.Users.Any(u => u.Email == email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas sprawdzania istnienia użytkownika: {Email}", email);
                return false;
            }
        }

        public User CreateUser(string email, string hashPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(hashPassword))
            {
                throw new ArgumentException("Email i hasło są wymagane");
            }

            if (email.Length > 100)
            {
                throw new ArgumentException("Email jest za długi");
            }

            try
            {
                var user = new User
                {
                    Email = email,
                    Password = hashPassword
                };

                context.Users.Add(user);
                context.SaveChanges();

                logger.LogInformation("Utworzono nowego użytkownika: {Email}", email);
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas tworzenia użytkownika: {Email}", email);
                throw;
            }
        }

        public async Task<City?> ReadCityAsync(int cityId)
        {
            if (cityId <= 0)
            {
                logger.LogWarning("Próba odczytu miasta z nieprawidłowym ID: {CityId}", cityId);
                return null;
            }

            try
            {
                return await context.Cities.FindAsync(cityId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas odczytu miasta ID: {CityId}", cityId);
                return null;
            }
        }

        public async Task AddCityAsync(City city, int userId)
        {
            if (city == null)
            {
                throw new ArgumentNullException(nameof(city), "Miasto nie może być null");
            }

            if (userId <= 0)
            {
                throw new ArgumentException("ID użytkownika musi być większe od 0", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(city.Name))
            {
                throw new ArgumentException("Nazwa miasta jest wymagana", nameof(city));
            }

            if (city.Latitude < -90 || city.Latitude > 90)
            {
                throw new ArgumentException("Szerokość geograficzna musi być z zakresu -90 do 90", nameof(city));
            }

            if (city.Longitude < -180 || city.Longitude > 180)
            {
                throw new ArgumentException("Długość geograficzna musi być z zakresu -180 do 180", nameof(city));
            }

            try
            {
                User? user = await context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException($"Użytkownik o ID {userId} nie istnieje");
                }

                city.User = user;
                city.UserId = user.Id;
                city.Name = city.Name.Trim();
                city.CantryCode = city.CantryCode?.Trim().ToUpperInvariant() ?? "";

                context.Cities.Add(city);
                await context.SaveChangesAsync();

                logger.LogInformation("Dodano miasto {CityName} dla użytkownika {UserId}", city.Name, userId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas dodawania miasta {CityName} dla użytkownika {UserId}", city.Name, userId);
                throw;
            }
        }

        public async Task DeleteCityAsync(int cityId)
        {
            if (cityId <= 0)
            {
                throw new ArgumentException("ID miasta musi być większe od 0", nameof(cityId));
            }

            try
            {
                City? city = await context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);

                if (city != null)
                {
                    // Usuń również powiązane dane pogodowe
                    var weatherData = await context.WeatherData
                        .Where(w => w.CityId == cityId)
                        .ToListAsync();

                    context.WeatherData.RemoveRange(weatherData);
                    context.Cities.Remove(city);
                    await context.SaveChangesAsync();

                    logger.LogInformation("Usunięto miasto ID: {CityId} wraz z danymi pogodowymi", cityId);
                }
                else
                {
                    logger.LogWarning("Próba usunięcia nieistniejącego miasta ID: {CityId}", cityId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas usuwania miasta ID: {CityId}", cityId);
                throw;
            }
        }

        public async Task<WeatherData?> RetriveWheaterDataAsync(int cityId, DateTime date)
        {
            if (cityId <= 0)
            {
                logger.LogWarning("Próba pobrania danych pogodowych z nieprawidłowym ID miasta: {CityId}", cityId);
                return null;
            }

            try
            {
                return await context.WeatherData
                    .Where(w => w.CityId == cityId && w.Date.Date == date.Date)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas pobierania danych pogodowych dla miasta {CityId} na dzień {Date}", cityId, date);
                return null;
            }
        }

        public async Task<List<WeatherData>> RetriveWheaterDataAsync(int cityId, DateTime begin, DateTime end)
        {
            if (cityId <= 0)
            {
                logger.LogWarning("Próba pobrania danych pogodowych z nieprawidłowym ID miasta: {CityId}", cityId);
                return new List<WeatherData>();
            }

            if (begin > end)
            {
                logger.LogWarning("Data początkowa jest późniejsza niż końcowa: {Begin} > {End}", begin, end);
                return new List<WeatherData>();
            }

            try
            {
                return await context.WeatherData
                    .Where(w => w.CityId == cityId && begin <= w.Date && w.Date < end)
                    .OrderBy(w => w.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas pobierania danych pogodowych dla miasta {CityId} w okresie {Begin} - {End}", cityId, begin, end);
                return new List<WeatherData>();
            }
        }

        public async Task AddWheaterAsync(WeatherData weather)
        {
            if (weather == null)
            {
                throw new ArgumentNullException(nameof(weather), "Dane pogodowe nie mogą być null");
            }

            if (weather.CityId <= 0)
            {
                throw new ArgumentException("ID miasta musi być większe od 0", nameof(weather));
            }

            try
            {
                // Walidacja danych pogodowych
                if (weather.Temperature < -100 || weather.Temperature > 100)
                {
                    logger.LogWarning("Podejrzana temperatura: {Temperature}°C dla miasta {CityId}", weather.Temperature, weather.CityId);
                }

                if (weather.Humidity < 0 || weather.Humidity > 100)
                {
                    weather.Humidity = Math.Max(0, Math.Min(100, weather.Humidity));
                }

                if (weather.Pressure < 0)
                {
                    weather.Pressure = 0;
                }

                if (weather.WindSpeed < 0)
                {
                    weather.WindSpeed = 0;
                }

                context.WeatherData.Add(weather);
                await context.SaveChangesAsync();

                logger.LogDebug("Dodano dane pogodowe dla miasta {CityId} na dzień {Date}", weather.CityId, weather.Date);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas dodawania danych pogodowych dla miasta {CityId}", weather.CityId);
                throw;
            }
        }

        public async Task UpdateWeatherAsync(WeatherData weather)
        {
            if (weather == null)
            {
                throw new ArgumentNullException(nameof(weather), "Dane pogodowe nie mogą być null");
            }

            if (weather.Id <= 0)
            {
                throw new ArgumentException("ID danych pogodowych musi być większe od 0", nameof(weather));
            }

            try
            {
                var existingWeather = await context.WeatherData.FindAsync(weather.Id);

                if (existingWeather != null)
                {
                    // Walidacja przed aktualizacją
                    if (weather.Humidity < 0 || weather.Humidity > 100)
                    {
                        weather.Humidity = Math.Max(0, Math.Min(100, weather.Humidity));
                    }

                    if (weather.Pressure < 0)
                    {
                        weather.Pressure = 0;
                    }

                    if (weather.WindSpeed < 0)
                    {
                        weather.WindSpeed = 0;
                    }

                    context.Entry(existingWeather).CurrentValues.SetValues(weather);
                    await context.SaveChangesAsync();

                    logger.LogDebug("Zaktualizowano dane pogodowe ID: {WeatherId}", weather.Id);
                }
                else
                {
                    logger.LogWarning("Próba aktualizacji nieistniejących danych pogodowych ID: {WeatherId}", weather.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Błąd podczas aktualizacji danych pogodowych ID: {WeatherId}", weather.Id);
                throw;
            }
        }
    }
}