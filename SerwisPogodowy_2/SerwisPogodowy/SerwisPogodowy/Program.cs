using SerwisPogodowy.DataBase;
using SerwisPogodowy.Repositories;
using SerwisPogodowy.Service;
using SerwisPogodowy.Models.Configuration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Dodaj API controllers (potrzebne dla Swaggera)
builder.Services.AddControllers();

// Konfiguracja Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Serwis Pogodowy API",
        Version = "v1",
        Description = "API dla serwisu pogodowego"
    });
});

// NOWE: Rejestracja konfiguracji WeatherApi
builder.Services.Configure<WeatherApiSettings>(
    builder.Configuration.GetSection("WeatherApi"));

builder.Services.AddDbContext<DataBaseContext>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IDataBaseRepository, DataBaseRepository>();
builder.Services.AddTransient<IRemoteWeatherRepository, RemoteWeatherRepository>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ICityService, CityService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Dodaj Swagger tylko w trybie deweloperskim
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Serwis Pogodowy API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();