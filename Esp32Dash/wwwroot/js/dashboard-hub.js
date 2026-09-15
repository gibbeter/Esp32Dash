const dhtTemperatureElement = document.getElementById("dhtTemperatureValue");
const dhtHumidityElement = document.getElementById("dhtHumidityValue");
const bmeTemperatureElement = document.getElementById("bmeTemperatureValue");
const bmeHumidityElement = document.getElementById("bmeHumidityValue");
//const ledElement = document.getElementById("ledValue");
const updateTimeElement = document.getElementById("updateTime");
const connectionIndicator = document.getElementById("connectionIndicator");

const ctx = document.getElementById("sensorChart").getContext("2d");

const sensorChart = new Chart(ctx, {
    type: "line",
    data: {
        labels: [],           // Will hold arrays like ["Jan 15", "14:30"]
        datasets: [
            {
                label: "DHT Temp (°C)",
                borderColor: "#FF6384",
                backgroundColor: "rgba(255, 99, 132, 0.1)",
                data: [],
                tension: 0.3,
                pointRadius: 4
            },
            {
                label: "BME Temp (°C)",
                
                borderColor: "#36A2EB",
                backgroundColor: "rgba(54, 162, 235, 0.1)",
                data: [],
                tension: 0.3,
                pointRadius: 4
            },
            {
                label: "DHT Hum (%)",
                borderColor: "#FFCE56",
                backgroundColor: "rgba(255, 206, 86, 0.1)",
                data: [],
                tension: 0.3,
                pointRadius: 4,
                borderDash: [5, 5]
            },
            {
                label: "BME Hum (%)",
                borderColor: "#4BC0C0",
                backgroundColor: "rgba(75, 192, 192, 0.1)",
                data: [],
                tension: 0.3,
                pointRadius: 4,
                borderDash: [5, 5]
            }
        ]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: "bottom",
                labels: {
                    boxWidth: 15,
                    padding: 15
                }
            }
        },
        scales: {
            y: {
                beginAtZero: false,
                title: {
                    display: true,
                    text: "Value"
                }
            },
            x: {
                title: {
                    display: true,
                    text: "Date / Time"
                },
                ticks: {
                    maxTicksLimit: 6,
                    maxRotation: 0,
                    minRotation: 0,
                    callback: function (value, index) {
                        const label = this.getLabelForValue(index);
                        return label;
                    }
                }
            }
        }
    },
    plugins: [{
        id: 'multilineLabels',
        afterCalculateTickRotation(scale) {
            scale.options.ticks.autoSkip = false;
        }
    }]
});

function formatLabel(isoTimestamp) {
    const date = new Date(isoTimestamp);
    const dateStr = date.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric'
    });
    const timeStr = date.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
    });

    return [dateStr, timeStr];
}

function addDataToChart(data) {
    const label = formatLabel(data.timestamp);

    // Add new data
    sensorChart.data.labels.push(label);
    sensorChart.data.datasets[0].data.push(data.dhtTemperature);
    sensorChart.data.datasets[1].data.push(data.bmeTemperature);
    sensorChart.data.datasets[2].data.push(data.dhtHumidity);
    // sensorChart.data.datasets[2].data.push(data.bmeTemperature);
    sensorChart.data.datasets[3].data.push(data.bmeHumidity);

    // Keep last 10 points
    if (sensorChart.data.labels.length > 10) {
        sensorChart.data.labels.shift();
        sensorChart.data.datasets.forEach(dataset => dataset.data.shift());
    }

    sensorChart.update();
}

function loadChartFromHistory(readings) {
    // Clear data
    sensorChart.data.labels = [];
    sensorChart.data.datasets.forEach(dataset => dataset.data = []);

    // New data
    readings.forEach(reading => {
        const label = formatLabel(reading.timestamp);
        sensorChart.data.labels.push(label);
        sensorChart.data.datasets[0].data.push(reading.dhtTemperature);
        sensorChart.data.datasets[1].data.push(reading.bmeTemperature);
        sensorChart.data.datasets[2].data.push(reading.dhtHumidity);
        // sensorChart.data.datasets[2].data.push(reading.bmeTemperature);
        sensorChart.data.datasets[3].data.push(reading.bmeHumidity);
    });

    sensorChart.update();
}

async function loadLatestData() {
    try {
        updateTimeElement.textContent = "Loading latest data...";

        const response = await fetch("/api/latest");

        if (response.ok) {
            const data = await response.json();
            updateDisplay(data);
            addDataToChart(data);
            updateTimeElement.textContent = "Last Update: " + new Date(data.timestamp).toLocaleString();
        } else if (response.status === 404) {
            updateTimeElement.textContent = "No data yet - waiting for ESP32...";
            dhtTemperatureElement.textContent = "--°C";
            dhtHumidityElement.textContent = "--%";
            dhtTemperatureElement.classList.remove("loading");
            dhtHumidityElement.classList.remove("loading");

            bmeTemperatureElement.textContent = "--°C";
            bmeHumidityElement.textContent = "--%";
            bmeTemperatureElement.classList.remove("loading");
            bmeHumidityElement.classList.remove("loading");
        } else {
            console.error("Failed to load data:", response.status);
            updateTimeElement.textContent = "Error loading data";
        }

        const recentResponse = await fetch("/api/recent");
        if (recentResponse.ok) {
            const recentData = await recentResponse.json();
            if (recentData.length > 0) {
                loadChartFromHistory(recentData);
            }
        }
    } catch (error) {
        console.error("Error fetching latest data:", error);
        updateTimeElement.textContent = "Cannot connect to server";
    }
}

function updateDisplay(data) {
    if (data.dhtTemperature !== undefined) {
        dhtTemperatureElement.textContent = data.dhtTemperature.toFixed(1) + "°C";
    }

    if (data.dhtHumidity !== undefined) {
        dhtHumidityElement.textContent = data.dhtHumidity.toFixed(1) + "%";
    }

    if (data.bmeTemperature !== undefined) {
        bmeTemperatureElement.textContent = data.bmeTemperature.toFixed(1) + "°C";
    }

    if (data.bmeHumidity !== undefined) {
        bmeHumidityElement.textContent = data.bmeHumidity.toFixed(1) + "%";
    }
}



// SignalR server url
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/sensorHub")
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveSensorData", function (data) {
    console.log("Received data:", data);

    updateDisplay(data);

    addDataToChart(data);

    const timestamp = data.timestamp ? new Date(data.timestamp) : new Date();
    updateTimeElement.textContent = "Last Update: " + timestamp.toLocaleTimeString();
});

// Connection lifecycle
connection.onreconnecting(function () {
    connectionIndicator.className = "status-dot disconnected";
    updateTimeElement.textContent = "Reconnecting...";
});

connection.onreconnected(function () {
    connectionIndicator.className = "status-dot connected";
    updateTimeElement.textContent = "Reconnected!";
});

connection.onclose(function () {
    connectionIndicator.className = "status-dot disconnected";
    updateTimeElement.textContent = "Connection lost";
});

async function initialize() {
    
    await loadLatestData();

    connection.start()
        .then(function () {
            console.log("SignalR Connected!");
            connectionIndicator.className = "status-dot connected";
        })
        .catch(function (err) {
            console.error("SignalR Connection Error:", err.toString());
            updateTimeElement.textContent = "Live updates unavailable: " + err.toString();
        });
}

initialize();