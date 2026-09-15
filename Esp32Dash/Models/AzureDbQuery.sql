CREATE TABLE [dbo].[SensorReadings] (
    [id]                INT              IDENTITY (1, 1) NOT NULL,
    [time_stamp]        DATETIME2        NOT NULL,
    [dht_temperature]   FLOAT            NOT NULL,
    [dht_humidity]      FLOAT            NOT NULL,
    [bme_temperature]   FLOAT            NOT NULL,
    [bme_humidity]      FLOAT            NOT NULL,
    [latitude]          FLOAT            NOT NULL,
    [longitude]         FLOAT            NOT NULL,
    [altitude]          FLOAT            NOT NULL,
    [speed]             FLOAT            NOT NULL,
    [satellites]        INT              NOT NULL,
    [city]              NVARCHAR(255)    NULL,
    [country]           NVARCHAR(255)    NULL,
    [isp]               NVARCHAR(255)    NULL,
    [ip_address]        NVARCHAR(45)     NULL,
    [location_source]   NVARCHAR(50)     NULL,
    CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED ([id] ASC)
);
GO

-- Add default constraints for numeric fields (optional)
ALTER TABLE [SensorReading] ADD CONSTRAINT DF_SensorData_Altitude DEFAULT 0 FOR [altitude];
ALTER TABLE [SensorReading] ADD CONSTRAINT DF_SensorData_Speed DEFAULT 0 FOR [speed];
ALTER TABLE [SensorReading] ADD CONSTRAINT DF_SensorData_Satellites DEFAULT 0 FOR [satellites];
GO