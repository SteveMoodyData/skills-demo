# Contoso.WeatherMeterService Architecture

## 1. Service Overview

- **Purpose and business function:** The service ingests two feeds and loads them into Oracle: weather observations pulled over HTTP from a weather API, and meter readings delivered as CSV files on a network share by a meter vendor. Weather values are converted to metric units and local time before load; meter readings are normalized and archived after processing.
- **Owner system / application context:** Contoso.WeatherMeterService (CONTOSO_WEATHERMETERSERVICE). Broader owner system: NOT FOUND IN CODE.
- **Trigger mechanism and schedule:** Two independent timers. Weather poll every 15 minutes; meter file poll every 60 minutes. After an error, the affected timer interval is set to 300 seconds.

## 2. Source Systems

| System Name | Protocol | Auth Method | Data Provided |
|---|---|---|---|
| Weather API (api.weather-demo.example) | HTTP | API key in X-Api-Key header (value redacted) | Weather observations JSON (per station) (FLOWS: 002) |
| Meter vendor | file drop | NOT FOUND IN CODE | Meter readings CSV (MeterId, ReadingDate, Kwh) (FLOWS: 001) |

## 3. Data Flow Summary

Two timers drive the service. On each weather tick, the service calls the weather API once per station in the configured station list, converts the returned values (UTC to Central Standard Time, Fahrenheit to Celsius, mph to m/s), and calls an Oracle procedure to store each observation. On each meter tick, the service reads every CSV in the inbound network share, uppercases the meter ID, defaults a blank kWh value to 0, calls an Oracle procedure to store each reading, and moves the file to an archive share. After processing it calls an Oracle purge procedure with a 30 day look-back. If either tick throws, the timer interval is changed to 300 seconds.

## 4. Transformation Logic

- Timestamp converted from UTC to Central Standard Time (FLOW-002)
- Temperature converted from Fahrenheit to Celsius, rounded to 1 decimal (FLOW-002)
- Wind speed converted from mph to m/s using factor 0.44704 (FLOW-002)
- MeterId trimmed and upper-cased (FLOW-001)
- Blank Kwh defaulted to 0 (FLOW-001)
- Source hardcoded to "FILE_DROP" (FLOW-001)

## 5. Oracle Integration

| Procedure Name | Schema | Target Table(s) | Purpose |
|---|---|---|---|
| WEATHER_PKG.INSERT_OBSERVATION | DEMO | NOT FOUND IN CODE | Store one weather observation (FLOWS: 002) |
| METER_PKG.INSERT_METER_READING | DEMO | NOT FOUND IN CODE | Store one meter reading (FLOWS: 001) |
| DELETE_PKG.PURGE_METER_READINGS | DEMO | NOT FOUND IN CODE | Purge meter readings older than 30 days (FLOWS: 001) |

## 6. Outbound Integrations

| Target System | SDK/Method | Data Pushed | Trigger |
|---|---|---|---|
| Meter archive share (\\fileshare.demo.example\meters\archive) | System.IO.File.Move | Processed meter CSV files | After readings are inserted (FLOW-001) |

## 7. Error Handling & Resilience

- On exception in either timer handler, the timer interval is set to 300 seconds (ErrorRetrySeconds).
- No dead-letter handling, logging, or per-record error isolation is indexed: NOT FOUND IN CODE.

## 8. Open Questions

- INSUFFICIENT EVIDENCE: target tables behind DEMO.WEATHER_PKG, DEMO.METER_PKG and DEMO.DELETE_PKG (package bodies not in repo).
- Auth method for the meter file share: NOT FOUND IN CODE.
- After an error the timer interval stays at 300 seconds; no reset to the normal interval is indexed.

## 9. Indexed Flows Referenced

CONTOSO_WEATHERMETERSERVICE-FLOW-001
CONTOSO_WEATHERMETERSERVICE-FLOW-002
