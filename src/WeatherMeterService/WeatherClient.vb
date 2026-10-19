Imports System.Configuration
Imports System.Net.Http
Imports Newtonsoft.Json.Linq

Public Class Observation
    Public Property StationId As String
    Public Property ObservedUtc As DateTime
    Public Property ObservedLocal As DateTime
    Public Property TempC As Decimal
    Public Property WindMps As Decimal
End Class

Public Class WeatherClient
    Public Function GetObservations(stationId As String) As List(Of Observation)
        Dim baseUrl As String = ConfigurationManager.AppSettings("WeatherApiBaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("WeatherApiKey")
        Dim url As String = baseUrl & "?station=" & stationId & "&hours=1"

        Using http As New HttpClient()
            http.DefaultRequestHeaders.Add("X-Api-Key", apiKey)
            Dim json As String = http.GetStringAsync(url).Result
            Dim root As JObject = JObject.Parse(json)
            Dim result As New List(Of Observation)

            For Each item As JObject In root("observations")
                Dim o As New Observation()
                o.StationId = stationId
                o.ObservedUtc = CDate(item("timestamp"))
                o.ObservedLocal = TimeZoneInfo.ConvertTimeFromUtc(o.ObservedUtc, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"))
                o.TempC = Math.Round((CDec(item("tempF")) - 32D) * 5D / 9D, 1)
                o.WindMps = CDec(item("windMph")) * 0.44704D
                result.Add(o)
            Next
            Return result
        End Using
    End Function
End Class
