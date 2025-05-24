using SerwisPogodowy.DataBase;
using SerwisPogodowy.Repositories;
using SerwisPogodowy.Service;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// DODANE: Dodaj kontrolery API
builder.Services.AddControllers();

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

// Konfiguracja Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Serwis Pogodowy API",
        Version = "v1",
        Description = "API do zarz¹dzania danymi pogodowymi i miastami u¿ytkowników"
    });

    // POPRAWIONE: Dodanie komentarzy XML z sprawdzeniem czy plik istnieje
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// POPRAWIONE: Swagger dostêpny zarówno w Development jak i innych œrodowiskach do testów
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Serwis Pogodowy API v1");
        c.RoutePrefix = "api-docs"; // ZMIENIONE: Standardowa œcie¿ka /swagger zamiast /api-docs
    });
}

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

// DODANE: Mapowanie kontrolerów API
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();