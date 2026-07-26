using Esp32Dash.Data;
using Microsoft.EntityFrameworkCore;

namespace Esp32Dashboard.Services;

public class SQLCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SQLCleanupService> _logger;

    private int MAX_RECORDS;
    private static TimeSpan CLEANUP_INTERVAL;

    public SQLCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<SQLCleanupService> logger,
    IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        MAX_RECORDS = configuration.GetValue<int>("Cleanup:MaxRecords", 10000);
        CLEANUP_INTERVAL = TimeSpan.FromHours(
            configuration.GetValue<int>("Cleanup:IntervalHours", 1));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SQL Cleanup Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldRecords();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during SQL data cleanup");
            }

            await Task.Delay(CLEANUP_INTERVAL, stoppingToken);
        }
    }

    private async Task CleanupOldRecords()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SensorDbContext>();
        var totalRecords = await db.SensorReadings.CountAsync();

        if (totalRecords > MAX_RECORDS)
        {
            var recordsToDelete = totalRecords - MAX_RECORDS;

            var oldestIds = await db.SensorReadings
                .OrderBy(s => s.Timestamp)
                .Take(recordsToDelete)
                .Select(s => s.Id)
                .ToListAsync();

            await db.SensorReadings
                .Where(s => oldestIds.Contains(s.Id))
                .ExecuteDeleteAsync();

            _logger.LogInformation("Deleted {Count} records to maintain max of {Max}",
                recordsToDelete, MAX_RECORDS);
        }

    }
}
