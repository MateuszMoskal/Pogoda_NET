# 🌤️ Serwis Pogodowy

Aplikacja webowa do zarządzania lokalizacjami i monitorowania pogody, stworzona w ASP.NET Core MVC z integracją OpenWeatherMap API.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?style=flat-square&logo=microsoft-sql-server)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=flat-square&logo=bootstrap)

## 📋 Spis treści

- [Funkcjonalności](#-funkcjonalności)
- [Technologie](#-technologie)
- [Wymagania](#-wymagania)
- [Instalacja](#-instalacja)
- [Konfiguracja](#-konfiguracja)
- [Struktura projektu](#-struktura-projektu)
- [API Documentation](#-api-documentation)
- [Bezpieczeństwo](#-bezpieczeństwo)
- [Troubleshooting](#-troubleshooting)
- [Rozwój](#-rozwój)
- [Licencja](#-licencja)

## ✨ Funkcjonalności

### 👤 Zarządzanie użytkownikami
- **Rejestracja** - tworzenie nowych kont użytkowników
- **Logowanie** - uwierzytelnianie z hashowaniem haseł (SHA256)
- **Sesje** - zarządzanie sesjami użytkowników
- **Bezpieczeństwo** - ochrona CSRF, walidacja danych

### 🏙️ Zarządzanie miastami
- **Wyszukiwanie miast** - integracja z OpenWeatherMap Geocoding API
- **Dodawanie lokalizacji** - zapisywanie ulubionych miejsc
- **Usuwanie miast** - zarządzanie listą lokalizacji
- **Geolokalizacja** - obsługa współrzędnych geograficznych

### 🌡️ Dane pogodowe
- **Aktualna pogoda** - temperatura, wilgotność, ciśnienie, wiatr
- **Prognoza 7-dniowa** - szczegółowa prognoza pogody
- **Ikony pogodowe** - wizualne reprezentacje warunków
- **Caching** - optymalizacja zapytań do API
- **Automatyczne odświeżanie** - aktualizacja co 3 godziny

### 📊 Wizualizacja
- **Wykresy temperatur** - interaktywne wykresy Chart.js
- **Karty pogodowe** - responsywny interfejs Bootstrap
- **Tabele prognoz** - szczegółowe dane tabelaryczne
- **Responsive design** - dostosowanie do urządzeń mobilnych

### 🔌 API
- **Swagger UI** - dokumentacja API (tryb deweloperski)
- **REST endpoints** - pełne API dla wszystkich funkcji
- **JSON responses** - standardowe odpowiedzi API
- **Walidacja** - kompletna walidacja danych wejściowych

## 🛠️ Technologie

### Backend
- **Framework**: ASP.NET Core 9.0 MVC
- **ORM**: Entity Framework Core 9.0
- **Baza danych**: SQL Server
- **Architektura**: Repository Pattern + Service Layer
- **Security**: Data Annotations, Anti-forgery tokens

### Frontend
- **UI Framework**: Bootstrap 5
- **JavaScript**: Vanilla JS + Chart.js 2.9
- **Icons**: Feather Icons
- **Styling**: CSS3, Responsive design

### External APIs
- **OpenWeatherMap API**: 
  - Current Weather API
  - 5 Day Weather Forecast API
  - Geocoding API

### Development Tools
- **Documentation**: Swagger/OpenAPI 3.0
- **Validation**: ASP.NET Core Data Annotations
- **Logging**: ASP.NET Core Logging
- **Configuration**: appsettings.json

## 📋 Wymagania

### Wymagania systemowe
- **.NET 9.0 SDK** lub nowszy
- **SQL Server** (LocalDB, Express, lub pełna wersja)
- **Visual Studio 2022** lub **Visual Studio Code**
- **IIS Express** (wbudowany w Visual Studio)

### Konta zewnętrzne
- **OpenWeatherMap API Key** - [Zarejestruj się tutaj](https://openweathermap.org/api)

## 🚀 Instalacja

### 1. Klonowanie repozytorium
```bash
git clone https://github.com/MateuszMoskal/Pogoda_NET.git
cd serwis-pogodowy
```

### 2. Instalacja zależności
```bash
dotnet restore
```

### 3. Konfiguracja bazy danych

#### Opcja A: SQL Server LocalDB (zalecane dla rozwoju)
```bash
# Sprawdź czy LocalDB jest zainstalowany
sqllocaldb info

# Utwórz instancję (jeśli nie istnieje)
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

#### Opcja B: SQL Server Express/Pełny
Zaktualizuj connection string w `DataBaseContext.cs`:
```csharp
string sqlServerName = "TwojSerwer\\SQLEXPRESS"; // lub inna instancja
```

### 4. Migracje bazy danych
```bash
# Dodaj migrację (jeśli nie istnieje)
dotnet ef migrations add InitialCreate

# Zaktualizuj bazę danych
dotnet ef database update
```

### 5. Konfiguracja API Key

Otwórz `Repositories/RemoteWeatherRepository.cs` i zastąp:
```csharp
private const string API_CODE = "TWOJ_OPENWEATHERMAP_API_KEY";
```

### 6. Uruchomienie aplikacji
```bash
dotnet run
```

Aplikacja będzie dostępna pod:
- **HTTPS**: https://localhost:7106
- **HTTP**: http://localhost:5237
- **Swagger UI**: https://localhost:7106/swagger (tylko development)

## ⚙️ Konfiguracja

### Struktura konfiguracji

#### appsettings.json (zalecane ulepszenie)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WeatherService;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "WeatherApi": {
    "ApiKey": "TWOJ_API_KEY",
    "BaseUrl": "https://api.openweathermap.org",
    "CacheTimeoutMinutes": 180
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Zmienne środowiskowe
```bash
# Dla produkcji - ustaw zmienne środowiskowe
export ASPNETCORE_ENVIRONMENT=Production
export WeatherApi__ApiKey=twoj_api_key
export ConnectionStrings__DefaultConnection=twoj_connection_string
```

### Konfiguracja sesji
```csharp
// Program.cs - aktualna konfiguracja
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

## 📁 Struktura projektu

```
SerwisPogodowy/
├── 📁 Controllers/           # Kontrolery MVC i API
│   ├── 📁 Api/              # Kontrolery API (Swagger)
│   │   ├── CityApiController.cs
│   │   ├── HomeApiController.cs
│   │   └── UserApiController.cs
│   ├── CityController.cs     # Kontroler miast (MVC)
│   ├── HomeController.cs     # Kontroler główny
│   └── UserController.cs     # Kontroler użytkowników
├── 📁 Models/               # Modele danych
│   ├── 📁 ViewModels/       # Modele widoków
│   │   ├── CitySearchVM.cs
│   │   ├── CityWheaterInformationVM.cs
│   │   ├── UserLoginVM.cs
│   │   ├── UserRegisterVM.cs
│   │   ├── WheaterForecastVM.cs
│   │   └── WheaterVM.cs
│   ├── City.cs              # Model miasta
│   ├── User.cs              # Model użytkownika
│   ├── WeatherData.cs       # Model danych pogodowych
│   ├── SessionUser.cs       # Model sesji użytkownika
│   └── ErrorViewModel.cs    # Model błędów
├── 📁 Views/                # Widoki Razor
│   ├── 📁 City/             # Widoki miast
│   │   ├── Add.cshtml       # Dodawanie miasta
│   │   ├── Index.cshtml     # Lista miast
│   │   └── WeatherForecast.cshtml # Prognoza
│   ├── 📁 User/             # Widoki użytkownika
│   │   ├── LogIn.cshtml     # Logowanie
│   │   └── Register.cshtml  # Rejestracja
│   ├── 📁 Home/             # Widoki główne
│   └── 📁 Shared/           # Współdzielone widoki
├── 📁 Service/              # Warstwa serwisów
│   ├── CityService.cs       # Serwis miast
│   ├── UserService.cs       # Serwis użytkowników
│   ├── SessionService.cs    # Serwis sesji
│   └── I*.cs               # Interfejsy serwisów
├── 📁 Repositories/         # Warstwa dostępu do danych
│   ├── DataBaseRepository.cs # Repository bazy danych
│   ├── RemoteWeatherRepository.cs # Repository API pogodowego
│   └── I*.cs               # Interfejsy repositories
├── 📁 DataBase/            # Kontekst bazy danych
│   └── DataBaseContext.cs   # EF Core DbContext
├── 📁 wwwroot/             # Pliki statyczne
│   ├── 📁 css/             # Style CSS
│   ├── 📁 js/              # JavaScript
│   ├── 📁 lib/             # Biblioteki (Bootstrap, jQuery)
│   └── 📁 graphics/        # Obrazy i ikony
└── Program.cs              # Punkt wejścia aplikacji
```

## 📚 API Documentation

### Endpoints

#### 🏙️ Cities API
```http
GET    /api/cityapi                    # Lista miast użytkownika
GET    /api/cityapi/search/{cityName}  # Wyszukaj miasta
POST   /api/cityapi                    # Dodaj miasto
DELETE /api/cityapi/{id}               # Usuń miasto
GET    /api/cityapi/{id}/weather       # Prognoza pogody
```

#### 👤 Users API
```http
POST   /api/userapi/login              # Logowanie
POST   /api/userapi/logout             # Wylogowanie
POST   /api/userapi/register           # Rejestracja
GET    /api/userapi/current            # Aktualny użytkownik
GET    /api/userapi/status             # Status logowania
```

#### 🏠 System API
```http
GET    /api/homeapi/status             # Status aplikacji
GET    /api/homeapi/info               # Informacje o aplikacji
```

### Przykłady użycia

#### Wyszukiwanie miasta
```bash
curl -X GET "https://localhost:7106/api/cityapi/search/Warszawa" \
  -H "accept: application/json"
```

#### Dodawanie miasta
```bash
curl -X POST "https://localhost:7106/api/cityapi" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Warszawa",
    "countryCode": "PL",
    "latitude": 52.2297,
    "longitude": 21.0122
  }'
```

#### Logowanie
```bash
curl -X POST "https://localhost:7106/api/userapi/login" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "haslo123"
  }'
```

### Swagger UI
W trybie deweloperskim dostępne pod: `https://localhost:7106/swagger`

## 🔒 Bezpieczeństwo

### Zaimplementowane zabezpieczenia

#### Uwierzytelnianie i autoryzacja
- **Hashowanie haseł**: SHA256 z UTF-8 encoding
- **Sesje**: HttpOnly cookies, 10-minutowy timeout
- **Autoryzacja**: Sprawdzanie sesji przed dostępem do zasobów

#### Ochrona przed atakami
- **CSRF Protection**: `[ValidateAntiForgeryToken]` na formularzach POST
- **XSS Prevention**: Automatyczne encodowanie w Razor
- **SQL Injection**: Entity Framework Core (parametryzowane zapytania)
- **Input Validation**: Data Annotations + server-side validation

#### Walidacja danych
```csharp
// Przykład walidacji w ViewModel
[Required(ErrorMessage = "Email jest wymagany")]
[EmailAddress(ErrorMessage = "Nieprawidłowy format emaila")]
public string Email { get; set; }

[Required(ErrorMessage = "Hasło jest wymagane")]
[MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków")]
public string Password { get; set; }
```

### Zalecenia bezpieczeństwa

#### Dla produkcji
1. **HTTPS Only**: Wymuszenie połączeń szyfrowanych
2. **Silne hasła**: Implementacja polityki haseł
3. **Rate Limiting**: Ograniczenie zapytań API
4. **Logging**: Monitoring prób logowania
5. **API Key Security**: Przechowywanie w zmiennych środowiskowych

#### Przykład konfiguracji produkcyjnej
```csharp
// Program.cs - dodatkowe zabezpieczenia
if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

## 🐛 Troubleshooting

### Częste problemy i rozwiązania

#### 1. Błąd HTTP 405 (Method Not Allowed)
**Problem**: Niezgodność nazw metod w kontrolerze i widoku
```
Rozwiązanie: Sprawdź czy nazwy metod w Html.BeginForm() są zgodne z kontrolerem
```

#### 2. Błąd połączenia z bazą danych
**Problem**: `SqlException: Cannot open database`
```bash
# Sprawdź connection string w DataBaseContext.cs
# Upewnij się że SQL Server działa
net start MSSQL$SQLEXPRESS

# Zaktualizuj bazę danych
dotnet ef database update
```

#### 3. Błąd API OpenWeatherMap
**Problem**: `HTTP 401 Unauthorized`
```
Rozwiązanie: 
1. Sprawdź API key w RemoteWeatherRepository.cs
2. Upewnij się że klucz jest aktywny (może potrwać do 2h)
3. Sprawdź limity API na koncie OpenWeatherMap
```

#### 4. Błędy walidacji
**Problem**: Model validation errors
```csharp
// Sprawdź ModelState w kontrolerze
if (!ModelState.IsValid)
{
    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
    {
        Console.WriteLine(error.ErrorMessage);
    }
    return View(model);
}
```

#### 5. Problemy z sesjami
**Problem**: Użytkownik wylogowywany automatycznie
```csharp
// Zwiększ timeout sesji w Program.cs
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Zwiększ z 10 do 30
});
```

### Debugging

#### Logowanie
```csharp
// Włącz szczegółowe logowanie w appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "SerwisPogodowy": "Information"
    }
  }
}
```

#### Entity Framework
```bash
# Sprawdź migracje
dotnet ef migrations list

# Zobacz SQL generowany przez EF
dotnet ef dbcontext info

# Usuń i utwórz bazę od nowa (tylko development!)
dotnet ef database drop
dotnet ef database update
```





<div align="center">





</div># Pogoda_NET