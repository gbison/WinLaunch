Imports System.IO
Imports System.Text


' equivelant to static class.

NotInheritable Class Storage
    Public Shared AppList As List(Of String)
    Public Shared FILENAME As String = "AppList.dat"
    Public Shared FILEPATH As String = Directory.GetCurrentDirectory()

    Private Sub New()
        'no instantiation
    End Sub

    Private Shared Function CollectApps()
        For Each app As App In Form1.apps
            Try
                Console.writeline(app.FileName)
                'AppList.Add(app.FileName)

            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
        Next
        Return True
    End Function

    Public Shared Function WriteData(ByRef appList as List(of App))
        ' write a list of all the data paths that the user has added.
        'CollectApps() 'gather apps.

        Dim fs As FileStream
        fs = IO.File.Create(FILEPATH + FILENAME)


        Dim pathBuffer as Byte()
        Try
            For Each app As App in appList
                console.writeline(app.FileName)
     
                pathBuffer = new UTF8Encoding(True).GetBytes(app.FileName + Environment.NewLine)
                fs.Write(pathBuffer, 0, pathBuffer.Length)
            Next

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return false
        End Try

        Threading.Thread.Sleep(1000)

        fs.Close()

        If File.Exists(FILEPATH + FILENAME) Then
            Console.WriteLine("The file was created successfully...")
        End If

        Return true
    End Function

    Public Shared Function ParseData()
        Try
            if IO.FILE.Exists(FILEPATH+FILENAME) then
                'load the file.
                Dim Reader As StreamReader
                Reader = New System.IO.StreamReader(FILEPATH+FILENAME)

                Dim tempstring As String

                Do
                    tempstring = Reader.ReadLine()
                    If Not tempstring = ""
                        Form1.AddAppFromFile(tempstring)
                    End If

                Loop Until tempstring = ""

                Reader.Close()
                
            End If
            Return True
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return False
        End Try

        
    End Function
End Class
