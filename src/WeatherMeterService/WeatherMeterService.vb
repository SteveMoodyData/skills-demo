Imports System.Configuration
Imports System.ServiceProcess
Imports System.Timers

''' <summary>
''' Windows service that ingests weather observations (HTTP API) and
''' meter readings (CSV file drop) and loads them into Oracle.
''' </summary>
Public Class WeatherMeterService
    Inherits ServiceBase

    Private WithEvents _weatherTimer As Timer
    Private WithEvents _meterTimer As Timer
    Private ReadOnly _db As New DemoDb()

    Protected Overrides Sub OnStart(args() As String)
        Dim weatherMinutes As Integer = CInt(ConfigurationManager.AppSettings("WeatherPollMinutes"))
        Dim meterMinutes As Integer = CInt(ConfigurationManager.AppSettings("MeterFilePollMinutes"))

        _weatherTimer = New Timer(weatherMinutes * 60 * 1000)
        _weatherTimer.Start()

        _meterTimer = New Timer(meterMinutes * 60 * 1000)
        _meterTimer.Start()
    End Sub

    Private Sub OnWeatherTick(sender As Object, e As ElapsedEventArgs) Handles _weatherTimer.Elapsed
        Try
            Dim client As New WeatherClient()
            Dim stations As String() = ConfigurationManager.AppSettings("StationList").Split(","c)
            For Each station In stations
                Dim obs As List(Of Observation) = client.GetObservations(station)
                For Each o In obs
                    _db.InsertObservation(o)
                Next
            Next
        Catch ex As Exception
            _weatherTimer.Interval = CInt(ConfigurationManager.AppSettings("ErrorRetrySeconds")) * 1000
        End Try
    End Sub

    Private Sub OnMeterTick(sender As Object, e As ElapsedEventArgs) Handles _meterTimer.Elapsed
        Try
            Dim processor As New MeterFileProcessor(_db)
            processor.ProcessInbound()
            _db.PurgeStaleReadings(30)
        Catch ex As Exception
            _meterTimer.Interval = CInt(ConfigurationManager.AppSettings("ErrorRetrySeconds")) * 1000
        End Try
    End Sub
End Class
