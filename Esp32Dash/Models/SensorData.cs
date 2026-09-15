// using System.ComponentModel.DataAnnotations.Schema;

// namespace Esp32Dash.Models
// {
//     public class SensorData
//     {
//         [Column("id")]
//         public int Id { get; set; }

//         [Column("time_stamp")]
//         public DateTime Timestamp { get; set; } = DateTime.UtcNow;

//         [Column("dht_temperature")]
//         public double dhtTemperature { get; set; }

//         [Column("dht_humidity")]
//         public double dhtHumidity { get; set; }

//         [Column("bme_temperature")]
//         public double bmeTemperature { get; set; }

//         [Column("bme_humidity")]
//         public double bmeHumidity { get; set; }

//     }
// }


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

        // GPS / Geolocation fields
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public int Satellites { get; set; }
        // [Column("latitude")]
        // public double Latitude { get; set; }

        // [Column("longitude")]
        // public double Longitude { get; set; }

        // [Column("altitude")]
        // public double Altitude { get; set; }          // 0 if not available

        // [Column("speed")]
        // public double Speed { get; set; }              // 0 if not available

        // [Column("satellites")]
        // public int Satellites { get; set; }            // 0 if not applicable

        // [Column("city")]
        // public string? City { get; set; }              // nullable string

        // [Column("country")]
        // public string? Country { get; set; }

        // [Column("isp")]
        // public string? Isp { get; set; }

        // [Column("ip_address")]
        // public string? IpAddress { get; set; }

        // [Column("location_source")]
        // public string? LocationSource { get; set; }    // e.g., "ip_geolocation"
    }
}