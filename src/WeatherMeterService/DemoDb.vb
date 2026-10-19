Imports System.Configuration
Imports System.Data
Imports Oracle.ManagedDataAccess.Client

''' <summary>Oracle access wrapper used by the service.</summary>
Public Class DemoDb
    Private ReadOnly _connString As String = ConfigurationManager.AppSettings("OracleConnection")

    Public Sub InsertObservation(o As Observation)
        Using conn As New OracleConnection(_connString)
            Using cmd As New OracleCommand("DEMO.WEATHER_PKG.INSERT_OBSERVATION", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("P_STATION_ID", o.StationId)
                cmd.Parameters.Add("P_OBSERVED_UTC", o.ObservedUtc)
                cmd.Parameters.Add("P_OBSERVED_LOCAL", o.ObservedLocal)
                cmd.Parameters.Add("P_TEMP_C", o.TempC)
                cmd.Parameters.Add("P_WIND_MPS", o.WindMps)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub InsertMeterReading(r As MeterReading)
        Using conn As New OracleConnection(_connString)
            Using cmd As New OracleCommand("DEMO.METER_PKG.INSERT_METER_READING", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("P_METER_ID", r.MeterId)
                cmd.Parameters.Add("P_READING_DATE", r.ReadingDate)
                cmd.Parameters.Add("P_KWH", r.Kwh)
                cmd.Parameters.Add("P_SOURCE", r.Source)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub PurgeStaleReadings(daysBack As Integer)
        Using conn As New OracleConnection(_connString)
            Using cmd As New OracleCommand("DEMO.DELETE_PKG.PURGE_METER_READINGS", conn)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add("P_DAYS_BACK", daysBack)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
End Class
