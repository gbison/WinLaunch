' Name: Form1.vb
' Description: Main code execution area for the application launcher platform.  This
' form class acts as the controller for the application.
'
'
' Date: 9/8/2018
' Programmer: Bryan Ison
Imports System.IO
Imports System.Text
Imports System.Windows.Forms.VisualStyles
Imports LauncherApp.App
Imports LauncherApp.Storage

Public Class Form1

    'maintain a list of the users apps.
    Public apps As New List(Of App)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'see if there is data
       if Storage.ParseData() = true
            'we found file.

            Console.WriteLine("File loaded successfully...")
        Else 
            Console.WriteLine("The file was NOT loaded....")

       End If 

    End Sub

    'path dialog
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles AddPath.Click

        Dim result As DialogResult = OpenFileDialog.ShowDialog()

        If result = DialogResult.OK Then
            'make sure we have room for it, columns and rows are 0 indexed.
            If TableLayout.Controls.Count <= TableLayout.ColumnCount * TableLayout.RowCount - 1 Then
                For i As Integer = 0 To TableLayout.RowCount - 1
                    For j As Integer = 0 To TableLayout.ColumnCount - 1
                        If TableLayout.GetControlFromPosition(j, i) Is Nothing Then
                            Try
                                Dim btn As Button = New Button()
                                TableLayout.Controls.Add(btn, j, i)
                                btn.Name = "Button" + i.ToString() + j.ToString()

                                'remove .exe from filename and cap the name
                                btn.Text = OpenFileDialog.SafeFileName.Remove(OpenFileDialog.SafeFileName.Length - 4).ToUpper()

                                'try to trap the icon
                                Dim icon As Icon = System.Drawing.Icon.ExtractAssociatedIcon(OpenFileDialog.FileName)

                                If Not icon Is Nothing Then
                                    btn.Image = icon.ToBitmap()
                                    btn.TextAlign = Drawing.ContentAlignment.BottomCenter
                                End If


                                btn.Anchor = AnchorStyles.None
                                btn.Dock = DockStyle.Fill 'make the button expand to the size of the table cells.
                                btn.FlatStyle = FlatStyle.Flat
                                btn.BackColor = Color.WhiteSmoke

                                AddHandler btn.Click, AddressOf Btn_Click   'assign the event handler for registering button clicks.
                                btn.ContextMenuStrip = ContextMenuStrip1    ' assign context menu to each button.
                                Dim menuItem As ToolStripMenuItem = New ToolStripMenuItem(btn.Text)
                                AddHandler menuItem.Click, AddressOf OnMenuItemClick

                                ContextMenuTrayIcon.Items.Add(menuItem)

                                '"AppsToolStripMenuItem"

                                apps.Add(New App(OpenFileDialog.FileName, btn)) 'store the file and the button reference in an app class object (App.vb)
                                Console.WriteLine("Selected Path {0}", OpenFileDialog.FileName)
                                MessageBox.Show("Added: " + OpenFileDialog.FileName, "SUCCESS")

                                Exit Sub 'bail on success.
                            Catch err As Exception
                                Console.WriteLine("Error Result: {0}", err) 'null ref exception is most likely to happen here.
                                MessageBox.Show(err.Message, "ERROR")
                            End Try
                        End If
                    Next
                Next
            Else
                MessageBox.Show("Unfortunately you cannot add anymore apps at this time. Please remove some current apps first....")
            End If
        End If
    End Sub

    Private Function OnMenuItemClick(sender As Object, e As EventArgs)

        Dim obj As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        ExecuteButton(obj)

        return true
    End Function

    'exit button
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        ExitApplication()

    End Sub

    Function Btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        'Form2.Show() ' hide the confirmation box, we dont really need it but leaving code for future.
        ExecuteButton(sender)
        Form2.CallingButton = sender
        return true
    End Function

    Private Sub ContextMenuStrip1_Click(sender As Object, e As EventArgs) Handles ContextMenuStrip1.Click

        RemoveAppControl(ContextMenuStrip1.SourceControl)

    End Sub

    Private Sub ShowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowToolStripMenuItem.Click
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.Focus()
        'Me.Refresh()
        Console.WriteLine("Window has been restored...")
    End Sub

    Private Sub Form1_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Hide()
            Console.WriteLine("Window has been minimized...")
        End If
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        ExitApplication()
    End Sub

    Overloads Sub Dispose()

        For Each control As Button In TableLayout.Controls
            RemoveHandler control.Click, AddressOf Btn_Click
        Next

    End Sub

    'helper functions 

    Public Function ExecuteButton(ByRef btn As Button)
        Try
            'this code allows me to be much more dynamic, such that I can use one event and no select statements and get the same result.
            'reach into the app object and find its button reference, if it matches the calling button, execute its File storage.
            For Each app As App In apps
                'find the button object that has been called.
                If app.ButtonRef.Name = btn.Name Then
                    Process.Start(app.FileName) 'now we can execute the file that belongs to that button.
                    Exit For 'bail once we found the calling button.
                End If
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            return false    
        End Try
        return true
    End Function

    'overrides previous for toolstrips and text comparison since tools tips hold item text not names.
    Public Function ExecuteButton(ByRef item As ToolStripMenuItem)
        Try
            'this code allows me to be much more dynamic, such that I can use one event and no select statements and get the same result.
            'reach into the app object and find its button reference, if it matches the calling button, execute its File storage.
            For Each app As App In apps
                'find the button object that has been called.
                If app.ButtonRef.Text = item.Text Then
                    Process.Start(app.FileName) 'now we can execute the file that belongs to that button.
                    Exit For 'bail once we found the calling button.
                End If
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return False
        End Try
        Return True
    End Function

    'remove the app and control references.
    Function RemoveAppControl(ByRef control As Control)

        RemoveApp(control)
        RemoveControl(control)
        Return true
    End Function

    'remove the app from our app group
    Function RemoveApp(ByRef control As Control)

        Try
            For Each obj As App In apps
                If obj.ButtonRef.Name = control.Name Then
                    apps.Remove(obj) 'remove the app first.
                    Exit For
                End If
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return False
        End Try
        Return True
    End Function

    'remove the control from the table layout.
    Function RemoveControl(ByRef control As Control)

        Try
            For Each obj As Button In TableLayout.Controls
                If obj.Name = control.Name Then
                    TableLayout.Controls.Remove(control)
                End If
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return False
        End Try
        Return True
    End Function

    Function ExitApplication()
        'save
        Storage.WriteData(apps) ' empty string allows defaults.

        Dispose()
        Me.Finalize()
        Application.Exit()
    End Function



    Public Function AddAppFromFile(appString as string)
        If TableLayout.Controls.Count <= TableLayout.ColumnCount * TableLayout.RowCount - 1 Then
                For i As Integer = 0 To TableLayout.RowCount - 1
                    For j As Integer = 0 To TableLayout.ColumnCount - 1
                        If TableLayout.GetControlFromPosition(j, i) Is Nothing Then
                            Try
                                Dim btn As Button = New Button()
                                TableLayout.Controls.Add(btn, j, i)
                                btn.Name = "Button" + i.ToString() + j.ToString()

                                ' since we dont have the dialog box to help us, we have to manully parse the name from the string.
                                Dim markerToSearchFor as String = "\" ' we know the string contains backslashes that signify directory depth.
                                Dim lastCharacter as Integer = appString.LastIndexOf(markerToSearchFor, StringComparison.Ordinal)+1
                                Dim subString as String = appString.Substring(lastCharacter, appString.Length-lastCharacter)
                                Console.WriteLine("Extracted filename: {0}", subString)
                            
                                btn.Text = subString.ToUpper()
                                'btn.Text = OpenFileDialog.SafeFileName.Remove(OpenFileDialog.SafeFileName.Length - 4).ToUpper()

                                'try to trap the icon
                                Dim icon As Icon = System.Drawing.Icon.ExtractAssociatedIcon(appString)

                                If Not icon Is Nothing Then
                                    btn.Image = icon.ToBitmap()
                                    btn.TextAlign = Drawing.ContentAlignment.BottomCenter
                                End If


                                btn.Anchor = AnchorStyles.None
                                btn.Dock = DockStyle.Fill 'make the button expand to the size of the table cells.
                                btn.FlatStyle = FlatStyle.Flat
                                btn.BackColor = Color.WhiteSmoke

                                AddHandler btn.Click, AddressOf Btn_Click   'assign the event handler for registering button clicks.
                                btn.ContextMenuStrip = ContextMenuStrip1    ' assign context menu to each button.
                                Dim menuItem As ToolStripMenuItem = New ToolStripMenuItem(btn.Text)
                                AddHandler menuItem.Click, AddressOf OnMenuItemClick

                                ContextMenuTrayIcon.Items.Add(menuItem)

                                '"AppsToolStripMenuItem"

                                apps.Add(New App(appString, btn)) 'store the file and the button reference in an app class object (App.vb)
                                Console.WriteLine("Selected Path {0}", appString)
                                'MessageBox.Show("Added: " + appString, "SUCCESS") REMOVED so the user doesnt get spammed on file load.

                                Exit Function 'bail on success.
                            Catch err As Exception
                                Console.WriteLine("Error Result: {0}", err) 'null ref exception is most likely to happen here.
                                MessageBox.Show(err.Message, "ERROR")
                            End Try
                        End If
                    Next
                Next
            Else
                MessageBox.Show("Unfortunately you cannot add anymore apps at this time. Please remove some current apps first....")
            End If
    End Function


End Class
