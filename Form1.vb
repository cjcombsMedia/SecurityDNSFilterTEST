Imports System.IO
Imports System.Net.Http

Public Class Form1
    Private WithEvents downloadTimer As New Timer()
    Private downloadTimeInMilliseconds As Integer
    Private httpClient As New HttpClient()
    Private stopTesting As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set up the ComboBox items
        ComboBox1.Items.AddRange(New String() {"5 Seconds", "10 Seconds", "20 Seconds", "30 Seconds", "1 Minute", "2 Minutes", "5 Minutes"})

        ' Set up the ListView columns
        ListView1.View = View.Details
        ListView1.Columns.Add("Status", 100)
        ListView1.Columns.Add("Time", 100)
        ListView1.Columns.Add("File Location", 200)

        ' Set up the timer interval based on the selected time in ComboBox
        downloadTimer.Interval = 1000 ' Default interval is 1 second
    End Sub

    Private Async Sub DownloadFileAsync(url As String)
        Try
            ' Create the directory if it doesn't exist
            Dim downloadDirectory As String = "C:\NetworkTesterFiles"
            If Not Directory.Exists(downloadDirectory) Then
                Directory.CreateDirectory(downloadDirectory)
            End If

            ' Create a unique file name to avoid conflicts
            Dim uniqueFileName As String = Guid.NewGuid().ToString()
            ' Set the file location for downloading
            Dim fileLocation As String = Path.Combine(downloadDirectory, uniqueFileName)

            ' Start the download and measure the time
            Dim startTime As DateTime = DateTime.Now
            Dim response As HttpResponseMessage = Await httpClient.GetAsync(url)
            Dim endTime As DateTime = DateTime.Now
            Dim downloadTimeInSeconds As Integer = CInt((endTime - startTime).TotalSeconds)

            ' Check if the download was successful
            If response.IsSuccessStatusCode Then
                Using stream As Stream = Await response.Content.ReadAsStreamAsync()
                    Using fileStream As FileStream = File.Create(fileLocation)
                        Await stream.CopyToAsync(fileStream)
                    End Using
                End Using

                ' Add an item to the ListView
                Dim item As New ListViewItem("Accessible")
                item.SubItems.Add(downloadTimeInSeconds.ToString())
                item.SubItems.Add(fileLocation)
                item.ForeColor = Color.Green
                ListView1.Items.Add(item)
            Else
                ' Add an item to the ListView indicating the failure
                Dim item As New ListViewItem("Filtered")
                item.SubItems.Add(downloadTimeInSeconds.ToString())
                item.SubItems.Add(fileLocation)
                item.ForeColor = Color.Red
                ListView1.Items.Add(item)
            End If
        Catch ex As Exception
            MessageBox.Show("Error occurred during download: " + ex.Message)
        End Try
    End Sub

    Private Sub downloadTimer_Tick(sender As Object, e As EventArgs) Handles downloadTimer.Tick
        ' Check if a URL is entered
        If Not String.IsNullOrEmpty(TextBox1.Text) Then
            ' Perform the download and update the ListView
            DownloadFileAsync(TextBox1.Text)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Check if a URL is entered
        If Not String.IsNullOrEmpty(TextBox1.Text) Then
            ' Start the testing
            StartTesting()
        Else
            MessageBox.Show("Please enter a URL.")
        End If
    End Sub

    Private Sub StartTesting()
        ' Reset the stopTesting flag
        stopTesting = False

        ' Disable the necessary controls
        ComboBox1.Enabled = False
        TextBox1.Enabled = False
        Button1.Enabled = False
        Button2.Enabled = True

        ' Clear the ListView
        ListView1.Items.Clear()

        ' Perform the initial download
        DownloadFileAsync(TextBox1.Text)

        ' Set the timer interval based on the selected time in ComboBox
        Select Case ComboBox1.SelectedItem.ToString()
            Case "5 Seconds"
                downloadTimeInMilliseconds = 5000
            Case "10 Seconds"
                downloadTimeInMilliseconds = 10000
            Case "20 Seconds"
                downloadTimeInMilliseconds = 20000
            Case "30 Seconds"
                downloadTimeInMilliseconds = 30000
            Case "1 Minute"
                downloadTimeInMilliseconds = 60000
            Case "2 Minutes"
                downloadTimeInMilliseconds = 120000
            Case "5 Minutes"
                downloadTimeInMilliseconds = 300000
        End Select

        ' Start the timer for subsequent downloads
        downloadTimer.Interval = downloadTimeInMilliseconds
        downloadTimer.Start()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Stop the testing
        stopTesting = True

        ' Stop the timer
        downloadTimer.Stop()

        ' Enable the necessary controls
        ComboBox1.Enabled = True
        TextBox1.Enabled = True
        Button1.Enabled = True
        Button2.Enabled = False

        ' Show a message box indicating that testing has stopped
        MessageBox.Show("Testing has stopped.")
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        ' No need for any code here
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Delete the downloaded files
        Dim downloadFolder As String = "C:\NetworkTesterFiles"
        If Directory.Exists(downloadFolder) Then
            Directory.Delete(downloadFolder, True)
        End If
    End Sub
End Class
