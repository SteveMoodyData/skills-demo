Imports System.Configuration
Imports System.IO

Public Class MeterReading
    Public Property MeterId As String
    Public Property ReadingDate As DateTime
    Public Property Kwh As Decimal
    Public Property Source As String
End Class

Public Class MeterFileProcessor
    Private ReadOnly _db As DemoDb

    Public Sub New(db As DemoDb)
        _db = db
    End Sub

    Public Sub ProcessInbound()
        Dim inboundPath As String = ConfigurationManager.AppSettings("MeterInboundPath")
        Dim archivePath As String = ConfigurationManager.AppSettings("MeterArchivePath")

        For Each file In Directory.GetFiles(inboundPath, "*.csv")
            For Each line In System.IO.File.ReadAllLines(file).Skip(1)
                Dim parts As String() = line.Split(","c)
                If parts.Length < 3 Then Continue For

                Dim r As New MeterReading()
                r.MeterId = parts(0).Trim().ToUpper()
                r.ReadingDate = DateTime.Parse(parts(1))
                r.Kwh = If(String.IsNullOrWhiteSpace(parts(2)), 0D, Decimal.Parse(parts(2)))
                r.Source = "FILE_DROP"
                _db.InsertMeterReading(r)
            Next
            System.IO.File.Move(file, Path.Combine(archivePath, Path.GetFileName(file)))
        Next
    End Sub
End Class
