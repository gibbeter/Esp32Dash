namespace Esp32Dash.Data
{
    using Microsoft.EntityFrameworkCore;

    public class SensorDbContext : DbContext
    {
        public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options) { }
        public DbSet<Models.SensorData> SensorReadings { get; set; }
    }
}
