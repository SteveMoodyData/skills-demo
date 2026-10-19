### FLOW INDEX (COPY INTO .github/skills/flow-index.md)
## CONTOSO_WEATHERMETERSERVICE-FLOW-001
service_name: Contoso.WeatherMeterService
service_key: CONTOSO_WEATHERMETERSERVICE
trigger_type: timer (System.Timers.Timer, _meterTimer)
ingestion_source: Meter vendor
ingestion_method: file drop
frequency: 60 minutes (MeterFilePollMinutes); on error: 300 seconds (ErrorRetrySeconds)
endpoint: \\fileshare.demo.example\meters\inbound (config key MeterInboundPath, *.csv)
external_calls: NOT FOUND IN CODE
interfaces_consumed:
 - interface_key: IF-FILE-UNC-fileshare.demo.example/meters/inbound
   interface_type: File drop (UNC)
   interface_name_or_location: \\fileshare.demo.example\meters\inbound
   evidence:
     - src/WeatherMeterService/MeterFileProcessor.vb:L19
     - src/WeatherMeterService/MeterFileProcessor.vb:L22
     - src/WeatherMeterService/App.config:L13
interfaces_produced:
 - interface_key: IF-ORACLE-PROC-DEMO.METER_PKG.INSERT_METER_READING
   interface_type: Oracle procedure
   interface_name_or_location: DEMO.METER_PKG.INSERT_METER_READING
   evidence:
     - src/WeatherMeterService/DemoDb.vb:L26
 - interface_key: IF-ORACLE-PROC-DEMO.DELETE_PKG.PURGE_METER_READINGS
   interface_type: Oracle procedure
   interface_name_or_location: DEMO.DELETE_PKG.PURGE_METER_READINGS
   evidence:
     - src/WeatherMeterService/DemoDb.vb:L40
     - src/WeatherMeterService/WeatherMeterService.vb:L46
 - interface_key: IF-FILE-UNC-fileshare.demo.example/meters/archive
   interface_type: File drop (UNC archive)
   interface_name_or_location: \\fileshare.demo.example\meters\archive
   evidence:
     - src/WeatherMeterService/MeterFileProcessor.vb:L34
     - src/WeatherMeterService/App.config:L14
inbound_feed_summary:
 - feed_key: FEED-01
   type: Meter readings CSV (MeterId, ReadingDate, Kwh)
   source_call: Directory.GetFiles(MeterInboundPath, "*.csv")
   source_call_filled: Directory.GetFiles("\\fileshare.demo.example\meters\inbound", "*.csv")
   frequency: 60 minutes
   evidence:
     - src/WeatherMeterService/MeterFileProcessor.vb:L22
evidence:
 - src/WeatherMeterService/WeatherMeterService.vb:L23
 - src/WeatherMeterService/WeatherMeterService.vb:L42-L50
 - src/WeatherMeterService/MeterFileProcessor.vb:L18-L36
 - src/WeatherMeterService/App.config:L6
notes:
 - Header row skipped (Skip(1)); rows with fewer than 3 columns are skipped
 - Empty Kwh value defaults to 0
 - Processed files are moved to the archive path
 - INSUFFICIENT EVIDENCE: target tables behind DEMO.METER_PKG / DEMO.DELETE_PKG (package bodies not in repo)
## CONTOSO_WEATHERMETERSERVICE-FLOW-002
service_name: Contoso.WeatherMeterService
service_key: CONTOSO_WEATHERMETERSERVICE
trigger_type: timer (System.Timers.Timer, _weatherTimer)
ingestion_source: Weather API
ingestion_method: HTTP
frequency: 15 minutes (WeatherPollMinutes); on error: 300 seconds (ErrorRetrySeconds)
endpoint: https://api.weather-demo.example/v1/observations?station={station}&hours=1 (config key WeatherApiBaseUrl)
external_calls: api.weather-demo.example
interfaces_consumed:
 - interface_key: IF-HTTP-api.weather-demo.example
   interface_type: HTTP REST API (JSON)
   interface_name_or_location: https://api.weather-demo.example/v1/observations
   evidence:
     - src/WeatherMeterService/WeatherClient.vb:L15
     - src/WeatherMeterService/WeatherClient.vb:L21
     - src/WeatherMeterService/App.config:L9
interfaces_produced:
 - interface_key: IF-ORACLE-PROC-DEMO.WEATHER_PKG.INSERT_OBSERVATION
   interface_type: Oracle procedure
   interface_name_or_location: DEMO.WEATHER_PKG.INSERT_OBSERVATION
   evidence:
     - src/WeatherMeterService/DemoDb.vb:L11
     - src/WeatherMeterService/WeatherMeterService.vb:L34
inbound_feed_summary:
 - feed_key: FEED-01
   type: Weather observations JSON (per station)
   source_call: GET {WeatherApiBaseUrl}?station={stationId}&hours=1 with X-Api-Key header
   source_call_filled: GET https://api.weather-demo.example/v1/observations?station={stationId}&hours=1 (REDACTED SECRET IN ENDPOINT)
   frequency: 15 minutes, once per station in StationList (STN001,STN002,STN003)
   evidence:
     - src/WeatherMeterService/WeatherClient.vb:L17
     - src/WeatherMeterService/App.config:L11
evidence:
 - src/WeatherMeterService/WeatherMeterService.vb:L20
 - src/WeatherMeterService/WeatherMeterService.vb:L27-L40
 - src/WeatherMeterService/WeatherClient.vb:L14-L36
 - src/WeatherMeterService/App.config:L5
notes:
 - Authentication is an API key sent in the X-Api-Key header (value redacted)
 - Timestamp converted UTC to Central Standard Time; Fahrenheit to Celsius; mph to m/s
 - INSUFFICIENT EVIDENCE: target table behind DEMO.WEATHER_PKG (package body not in repo)
