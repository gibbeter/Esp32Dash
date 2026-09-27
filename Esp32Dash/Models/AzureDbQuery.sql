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
    CONSTRAINT [PK_SensorData] PRIMARY KEY CLUSTERED ([id] ASC)
);

ALTER TABLE [SensorReadings] ADD CONSTRAINT DF_SensorData_Altitude DEFAULT 0 FOR [altitude];
ALTER TABLE [SensorReadings] ADD CONSTRAINT DF_SensorData_Speed DEFAULT 0 FOR [speed];
ALTER TABLE [SensorReadings] ADD CONSTRAINT DF_SensorData_Satellites DEFAULT 0 FOR [satellites];

CREATE NONCLUSTERED INDEX IX_SensorReadings_Timestamp
    ON [dbo].[SensorReadings] ([time_stamp] DESC);