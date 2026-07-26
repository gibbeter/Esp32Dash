using System.ComponentModel.DataAnnotations.Schema;

namespace Esp32Dash.Models
{
    public class SensorData
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("time_stamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Column("dht_temperature")]
        public double dhtTemperature { get; set; }

        [Column("dht_humidity")]
        public double dhtHumidity { get; set; }

        [Column("bme_temperature")]
        public double bmeTemperature { get; set; }

        [Column("bme_humidity")]
        public double bmeHumidity { get; set; }

    }
}
