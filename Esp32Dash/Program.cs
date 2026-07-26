using Esp32Dash.ApiMiddleware;
using Esp32Dash.Data;
using Esp32Dash.Hubs;
using Esp32Dash.Models;
using Esp32Dashboard.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Database Context (connection string)
builder.Services.AddDbContext<SensorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSqlConnection")));



// SignalR - live web page
builder.Services.AddSignalR();

// SQL cleaner
builder.Services.AddHostedService<SQLCleanupService>();

// Swagger registration
builder.Services.AddEndpointsApiExplorer();

// Swagger for testing
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ESP32 Dashboard API",
        Version = "v1",
        Description = "API for receiving ESP32 sensor data"
    });

    // API Key config
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access endpoints. Enter your key below.",
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Server static files (for the dashboard.html)
app.UseDefaultFiles();
app.UseStaticFiles();

// API key middleware
app.UseApiKey();

// SignalR Hub to request mapping
app.MapHub<SensorHub>("/sensorHub");

// Dashboard page
app.MapGet("/dashboard", () => Results.Redirect("/dashboard.html"));

// Latest readings from SQL
app.MapGet("/api/latest", async (SensorDbContext db) =>
{
    var latestReading = await db.SensorReadings
                                .OrderByDescending(s => s.Timestamp)
                                .FirstOrDefaultAsync();

    if (latestReading is null)
    {
        return Results.NotFound(new { message = "No data available" });
    }

    return Results.Ok(latestReading);
});

// Last 10 readings
app.MapGet("/api/recent", async (SensorDbContext db) =>
{
    var recentReadings = await db.SensorReadings
                            .OrderByDescending(s => s.Timestamp)
                            .Take(10)
                            .ToListAsync();

    var sorted = recentReadings.OrderBy(s => s.Timestamp).ToList();

    return Results.Ok(sorted);
});


// ESP32 data to DB/Web
app.MapPost("/data", async (SensorData incomingData, SensorDbContext db, IHubContext<SensorHub> hubContext) =>
{
    incomingData.Timestamp = DateTime.UtcNow;

    db.SensorReadings.Add(incomingData);
    await db.SaveChangesAsync();

    await hubContext.Clients.All.SendAsync("ReceiveSensorData", incomingData);

    return Results.Ok(new { status = "Data received and broadcasted" });
});

app.Run();