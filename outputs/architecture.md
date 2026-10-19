# Contoso.WeatherMeterService Architecture

## 1. Service Overview

- **Purpose and business function:** The indexed flows show the service ingesting meter readings from vendor CSV files and weather observations from an HTTP API, and producing Oracle procedure interactions for both feeds. (Evidence: `src/WeatherMeterService/MeterFileProcessor.vb:L22`; `src/WeatherMeterService/WeatherClient.vb:L15`; `src/WeatherMeterService/DemoDb.vb:L11`; `src/WeatherMeterService/DemoDb.vb:L26`.)
- **Owner system / application context:** Contoso.WeatherMeterService (CONTOSO_WEATHERMETERSERVICE). Broader owner system: NOT FOUND IN CODE. (Evidence: `.github/skills/flow-index.md` — `service_name: Contoso.WeatherMeterService`; `service_key: CONTOSO_WEATHERMETERSERVICE`.)
- **Trigger mechanism and schedule:** The meter flow uses a timer at 60-minute intervals, and the weather flow uses a timer at 15-minute intervals. Both indexed flows specify 300 seconds on error. (Evidence: `src/WeatherMeterService/WeatherMeterService.vb:L23`; `src/WeatherMeterService/WeatherMeterService.vb:L20`; flow-index frequency fields.)

## 2. Source Systems

| System Name | Protocol | Auth Method | Data Provided |
|---|---|---|---|
| Meter vendor | file drop | NOT FOUND IN CODE | Meter readings CSV (MeterId, ReadingDate, Kwh) (FLOWS: 001; evidence: `src/WeatherMeterService/MeterFileProcessor.vb:L22`) |
| Weather API (api.weather-demo.example) | HTTP | API key in X-Api-Key header (value redacted) | Weather observations JSON (per station) (FLOWS: 002; evidence: `src/WeatherMeterService/WeatherClient.vb:L17`; `src/WeatherMeterService/App.config:L11`) |

## 3. Data Flow Summary

The indexed flows are timer-triggered: the meter flow polls every 60 minutes and the weather flow every 15 minutes. The meter flow reads CSV files matching `*.csv` from `\\fileshare.demo.example\meters\inbound`; the weather flow requests observations from `https://api.weather-demo.example/v1/observations?station={station}&hours=1`, once per station in the indexed station list. The meter feed is identified as CSV fields MeterId, ReadingDate, and Kwh; its notes say the header row and rows with fewer than three columns are skipped and an empty Kwh defaults to 0. Weather-feed notes specify UTC-to-Central-Standard-Time, Fahrenheit-to-Celsius, and mph-to-m/s conversions. The indexed Oracle interfaces are `DEMO.METER_PKG.INSERT_METER_READING`, `DEMO.DELETE_PKG.PURGE_METER_READINGS`, and `DEMO.WEATHER_PKG.INSERT_OBSERVATION`. Processed meter files are moved to the indexed archive path `\\fileshare.demo.example\meters\archive`. Both flows specify 300 seconds on error. (Evidence: `src/WeatherMeterService/MeterFileProcessor.vb:L22`; `src/WeatherMeterService/WeatherClient.vb:L15`; `src/WeatherMeterService/App.config:L9`; `src/WeatherMeterService/App.config:L13`; `src/WeatherMeterService/App.config:L14`; `src/WeatherMeterService/DemoDb.vb:L11`; `src/WeatherMeterService/DemoDb.vb:L26`; `src/WeatherMeterService/DemoDb.vb:L40`; `src/WeatherMeterService/WeatherMeterService.vb:L46`.)

## 4. Transformation Logic

- Meter CSV header row skipped; rows with fewer than three columns skipped (FLOW-001; evidence: `src/WeatherMeterService/MeterFileProcessor.vb:L18-L36`).
- Empty Kwh value defaults to 0 (FLOW-001; evidence: `src/WeatherMeterService/MeterFileProcessor.vb:L18-L36`).
- Weather timestamp converted from UTC to Central Standard Time (FLOW-002; evidence: `src/WeatherMeterService/WeatherClient.vb:L14-L36`).
- Weather temperature converted from Fahrenheit to Celsius (FLOW-002; evidence: `src/WeatherMeterService/WeatherClient.vb:L14-L36`).
- Weather wind speed converted from mph to m/s (FLOW-002; evidence: `src/WeatherMeterService/WeatherClient.vb:L14-L36`).

## 5. Oracle Integration

| Procedure Name | Schema | Target Table(s) | Purpose |
|---|---|---|---|
| DEMO.METER_PKG.INSERT_METER_READING | DEMO | NOT FOUND IN CODE | Insert meter reading, as named by the indexed procedure interface (FLOWS: 001; evidence: `src/WeatherMeterService/DemoDb.vb:L26`) |
| DEMO.DELETE_PKG.PURGE_METER_READINGS | DEMO | NOT FOUND IN CODE | Purge meter readings, as named by the indexed procedure interface (FLOWS: 001; evidence: `src/WeatherMeterService/DemoDb.vb:L40`) |
| DEMO.WEATHER_PKG.INSERT_OBSERVATION | DEMO | NOT FOUND IN CODE | Insert weather observation, as named by the indexed procedure interface (FLOWS: 002; evidence: `src/WeatherMeterService/DemoDb.vb:L11`) |

## 6. Outbound Integrations

| Target System | SDK/Method | Data Pushed | Trigger |
|---|---|---|---|
| Meter archive share (`\\fileshare.demo.example\meters\archive`) | NOT FOUND IN CODE | Processed meter files (FLOW-001) | After processing (exact timing NOT FOUND IN CODE) |

## 7. Error Handling & Resilience

- Both flows specify 300 seconds on error (ErrorRetrySeconds), as indexed frequency behavior.
- Logging and dead-letter handling: NOT FOUND IN CODE.

## 8. Open Questions

- INSUFFICIENT EVIDENCE: target tables behind DEMO.METER_PKG, DEMO.DELETE_PKG, and DEMO.WEATHER_PKG; the flow index notes that package bodies are not present in the repository.
- Meter file-share authentication method: NOT FOUND IN CODE.
- Archive SDK/method and exact move timing: NOT FOUND IN CODE.
- Broader owner system: NOT FOUND IN CODE.

## 9. Indexed Flows Referenced

CONTOSO_WEATHERMETERSERVICE-FLOW-001
CONTOSO_WEATHERMETERSERVICE-FLOW-002
