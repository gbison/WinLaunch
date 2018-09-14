' Name: App.vb
' Description: Application data class for holding information in regard to the applications the user wants to execute.
'
'
' Date: 9/8/2018
' Programmer: Bryan Ison

Public Class App

    Dim m_fileLocation As String
    Dim m_fileExt As String
    Dim m_imageFile As String
    Dim m_buttonReference As Button

    'get and set the file location or root path the file will reside.
    Property FileName() As String
        Get
            Return m_fileLocation
        End Get
        Set(value As String)
            m_fileLocation = value
        End Set
    End Property

    'get and set the file extension of the class.
    Property FileExtension() As String
        Get
            Return m_fileExt
        End Get
        Set(value As String)
            m_fileExt = value
        End Set
    End Property

    Property ButtonRef() As Button
        Get
            Return m_buttonReference
        End Get
        Set(value As Button)
            m_buttonReference = value
        End Set
    End Property

    'require at least a path.
    Public Sub New(path As String, ByRef btn As Button)

        m_fileLocation = path
        m_fileExt = Nothing
        m_imageFile = Nothing
        m_buttonReference = btn

    End Sub

    'accept path and ext
    Public Sub New(path As String, btn As Button, ext As String)

        m_fileLocation = path
        m_fileExt = ext
        m_imageFile = Nothing

    End Sub

    Public Sub Dispose()

        Dispose()
        Me.Finalize()

    End Sub

End Class
