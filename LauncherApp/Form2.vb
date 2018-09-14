' Name: Form2.vb
' Description: Secondary form class for requiring confirmation from the user about
' the procedure they are trying to execute.
'
'
' Date: 9/8/2018
' Programmer: Bryan Ison

Public Class Form2
    Public CallingButton As Button ' passed from Form1 when the user clicks a specific button.

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ConfirmOK.Click
        If CheckBox1.Checked Then
            CheckRemoval() 'user wants to remove the app not execute it/
            Me.Hide()
            Exit Sub
        End If
        Form1.ExecuteButton(CallingButton)
        Me.Hide()

        'Label1.Text = 99 Mod 25
        'Console.WriteLine(Label1.Text)
        'Threading.Thread.Sleep(1000)
    End Sub

    Private Sub ConfirmCancel_Click(sender As Object, e As EventArgs) Handles ConfirmCancel.Click
        Me.Hide()
        CallingButton = Nothing
    End Sub

    Function CheckRemoval()

        Try
            'iterate through layout controls first and find a match.
            Console.WriteLine("Looking to remove control name: {0}", CallingButton)
            For Each control As Button In Form1.TableLayout.Controls
                If control.Name = CallingButton.Name Then
                    ' now that we have a match, iterate through the apps and find the one that was clicked.
                    For Each app As App In Form1.apps
                        If app.ButtonRef.Name = CallingButton.Name Then
                            Form1.apps.Remove(app) 'remove the app first.
                            Exit For
                        End If
                    Next
                    Form1.TableLayout.Controls.Remove(control) ' now remove the control
                    Console.WriteLine("Removing control: {0} ", control.Name.ToString())

                    Exit Function
                End If
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return False
        End Try

        Return True

    End Function


End Class