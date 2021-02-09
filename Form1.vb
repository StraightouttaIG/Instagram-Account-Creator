Imports System.ComponentModel
Imports System.Net
Imports System.Text

Public Class Form1
    Dim proxy_username As String
    Dim proxy_password As String
    Dim proxy_list As String()
    Dim desktop = My.Computer.FileSystem.SpecialDirectories.Desktop
    Dim fbid As String
    Dim sessions As String()
    Dim two_steps_code As Boolean = False
    Dim codes As String()
    Dim accounts As New StringBuilder
    Dim ls As Integer = 0


#Region "Form1 events"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Instagram account maker v2.0
        ' Fully programmed by NWA 

        ' Instagram: https://www.instagram.com/_u/_n8z/
        ' Discord: NWA#2076
        ' Discord  channel: https://discord.com/invite/npw9qha/

        Control.CheckForIllegalCrossThreadCalls = False
        Timer1.Start()
    End Sub


    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ' backup save
        If TextBox1.Lines.Count <> 0 Then
            Dim rndpath As String = desktop + $"\{New Random().Next(0, 99999).ToString}_users.txt"
            IO.File.WriteAllText(desktop + $"\{New Random().Next(0, 99999).ToString}_users.txt", TextBox1.Text)
            MsgBox("Exported to " + rndpath)
            If Not ListBox2.Items.Count < 1 Then
                export_btn.PerformClick()
            End If
        End If
        If ListBox1.Items.Count <> 0 Then
            Button2.PerformClick()
        End If
    End Sub
#End Region


#Region "Import/export"
    Private Sub import_tokens_Click(sender As Object, e As EventArgs) Handles import_tokens.Click
        Using ofd As OpenFileDialog = New OpenFileDialog
            ofd.InitialDirectory = "c:\"
            ofd.Filter = "All files|*.*|Text files|*.txt"
            If ofd.ShowDialog() <> DialogResult.Cancel Then
                Dim tokens As String() = IO.File.ReadAllLines(ofd.FileName)
                For Each line As String In tokens
                    ListBox1.Items.Add(line)
                Next
            End If
        End Using
    End Sub


    Private Sub export_btn_Click(sender As Object, e As EventArgs) Handles export_btn.Click
        Try
            Dim str As New StringBuilder
            For Each item As String In ListBox2.Items
                str.AppendLine(item)
            Next
            Dim rndpath As String = desktop + $"\{New Random().Next(0, 99999).ToString}_users.txt"
            IO.File.WriteAllText(desktop + $"\{New Random().Next(0, 99999).ToString}_users.txt", str.ToString)
            MsgBox("Exported to " + rndpath)
            Process.Start(rndpath)
        Catch ex As Exception
        End Try
    End Sub


    Private Sub import_codes_Click(sender As Object, e As EventArgs) Handles import_codes.Click
        Using ofd As OpenFileDialog = New OpenFileDialog
            ofd.InitialDirectory = "c:\"
            ofd.Filter = "All files|*.*|Text files|*.txt"
            If ofd.ShowDialog() <> DialogResult.Cancel Then
                codes = IO.File.ReadAllLines(ofd.FileName)
                two_steps_code = True
            End If
        End Using
        MsgBox(codes.Length.ToString + " Codes imported")
    End Sub
#End Region


    Private Sub start_btn_Click(sender As Object, e As EventArgs) Handles start_Btn.Click
        If two_steps_code = True Then
            Dim nwa As New Threading.Thread(AddressOf exchange_loop) : nwa.IsBackground = True : nwa.Start()
        Else
            Dim nwa As New Threading.Thread(AddressOf signup_loop) : nwa.IsBackground = True : nwa.Start()
        End If
    End Sub


    Private Sub useProxy_CheckedChanged(sender As Object, e As EventArgs) Handles useProxy.CheckedChanged
        If useProxy.Checked = True Then
            Try
                proxy_list = IO.File.ReadAllLines("proxies.txt")
                Dim cred As String = InputBox("username:password for proxy auth: ")
                If cred > "" Then
                    proxy_username = cred.Split(":")(0)
                    proxy_password = cred.Split(":")(1)
                Else
                    'none 
                End If
            Catch ex As Exception
                MsgBox("Add proxies to proxies.txt", MsgBoxStyle.Critical,)
                useProxy.Checked = False
            End Try
        End If
    End Sub


    Function random_string()
        Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToLower
        Dim r As New Random
        Dim sb As New StringBuilder
        For i As Integer = 1 To 10
            Dim idx As Integer = r.Next(0, 35)
            sb.Append(s.Substring(idx, 1))
        Next
        Return sb.ToString()
    End Function
    Function random_proxy() As WebProxy
        Try
            Dim proxy As New WebProxy(proxy_list(New Random().Next(0, proxy_list.Length - 1)))
            proxy.Credentials = New NetworkCredential(proxy_username, proxy_password)
            Return proxy
        Catch ex As Exception
        End Try
    End Function

#Region "Loops"
    Sub signup_loop()
        While Not ls >= ListBox1.Items.Count
            Try
                signup(ListBox1.Items(ls))
                Threading.Thread.Sleep(500)
            Catch ex As Exception
            End Try
        End While
        MsgBox("Import more tokens." + vbNewLine + "-- Stopped --")
    End Sub


    Sub exchange_loop()
        Try
            Parallel.ForEach(codes, Sub(code)
                                        code_exchange(code)
                                        Threading.Thread.Sleep(500)
                                    End Sub)
            Dim nwa As New Threading.Thread(AddressOf signup_loop) : nwa.IsBackground = True : nwa.Start()
        Catch ex As Exception
        End Try
    End Sub
#End Region


    Sub code_exchange(code As String)
        ' exchange facebook token to instagram token
        Try : Net.ServicePointManager.CheckCertificateRevocationList = False : Net.ServicePointManager.DefaultConnectionLimit = 300 : Net.ServicePointManager.UseNagleAlgorithm = False : Net.ServicePointManager.Expect100Continue = False : Net.ServicePointManager.SecurityProtocol = 3072
            Dim Encoding As New Text.UTF8Encoding
            Dim Bytes As Byte() = Encoding.GetBytes($"code={code}&returnURL=/")
            Dim AJ As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create("https://www.instagram.com/accounts/fb_code_exchange/"), Net.HttpWebRequest)
            If useProxy.CheckState = 1 Then
                AJ.Proxy = random_proxy()
            End If
            With AJ
                .Method = "POST"
                .Host = "www.instagram.com"
                .UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:85.0) Gecko/20100101 Firefox/85.0"
                .Accept = "*/*"
                .Headers.Add("Accept-Language: en-US,en;q=0.5")
                .AutomaticDecompression = Net.DecompressionMethods.Deflate Or Net.DecompressionMethods.GZip
                .Headers.Add("X-CSRFToken: ICHPF1irSgpUSiPYKEJFg5zHr3lkvj6C")
                .Headers.Add("X-Instagram-AJAX: 7a3a3e64fa87")
                .Headers.Add("X-IG-App-ID: 936619743392459")
                .Headers.Add("X-IG-WWW-Claim: 0")
                .ContentType = "application/x-www-form-urlencoded"
                .Headers.Add("X-Requested-With: XMLHttpRequest")
                .ContentLength = Bytes.Length
                .Headers.Add("Origin: https://www.instagram.com")
                .KeepAlive = True
                .Headers.Add("Cookie: ig_did=C0AB347D-B55A-42E2-A8EB-35706B6FA808; mid=YAWk4AALAAEAPEffLDD69yV6aY8Q; ig_nrcb=1; fbm_124024574287414=base_domain=.instagram.com; csrftoken=ICHPF1irSgpUSiPYKEJFg5zHr3lkvj6C; fbsr_124024574287414=68Cy92g6EhoDXDDIj1Qd9q58qRGNK3NGtpod5AhjQpE.eyJ1c2VyX2lkIjoiMTAwMDQxMTEzMTQ2MDI2IiwiY29kZSI6IkFRQ09MbHRxSVZMdFVnLVN0cEpmRm9yaXRaWlBCS2Z5Uk50Mm90Und2X1RmcWVHTU5RTGkwaTNxUU8zWnJqUHc1clZKYmtUMm9QUWRkem5LcEY4eWVLekpGSjJyeXJCYXhZNVEzQUZvZ3ZtRm1nbVVVemJHbW5EazNoMk9fQ1ZSZjMyMTk2aHBWVHZ3bEFCR3VmeXlUTjNmRVg5WUZ5ems3OXdqUTB5SzJHZHRKUHJwMVNlTDhIOVBWSEhBUFcweGtqYkF1X3FzVGlnTlN1Y0liOTRkS2xjaXVfa3ZiRS1HVktDUjJ2YUtjbTlBSi1jZzlSV2s1VFk4ZXczVkdIdEN5WnpVaFhaUGRjbWY4ZnVzVm9XcnE0WFN2bU43OHROWFZxRk9QSjN2U09CN1loRkNVbkl1cGxmalo5TnpxZ0d2YUNYeXV4d2ttOU1lRkhaak1fQ2ZUc3h2Wnh4eHBFVVdPNDFNWHpOb2tMcnhhUSIsIm9hdXRoX3Rva2VuIjoiRUFBQnd6TGl4bmpZQkFMU3JOMUxvbWhnWVhJcDNDMlpDd1pBbDFJYU41Ukg1TDk0bzdhQk9ucFd6S09oMk90RmxhcnJmYkVvUlFOWUtUbFVORjJETmM5R2M3TUpaQUFxalVPZUtRSndkOVBMM0lpWEl1TmRneEFjZFhDWkJ5aTZaQ2RsM0lRWkN4T0dRZHJvcVpDMFJPdW9BbTNWY3VIaUN0VnhEUjBXbGs5Y0NzOU5CSFpDUVkwM3Jya2gwa1FOZGF4b1pEIiwiYWxnb3JpdGhtIjoiSE1BQy1TSEEyNTYiLCJpc3N1ZWRfYXQiOjE2MTIxNjkxMDV9")
                .Headers.Add("Pragma: no-cache")
                .Headers.Add("Cache-Control: no-cache")
                .Headers.Add("TE: Trailers")
            End With
            Dim Stream As IO.Stream = AJ.GetRequestStream() : Stream.Write(Bytes, 0, Bytes.Length) : Stream.Dispose() : Stream.Close()
            Dim Reader As New IO.StreamReader(DirectCast(AJ.GetResponse(), Net.HttpWebResponse).GetResponseStream()) : Dim Text As String = Reader.ReadToEnd : Reader.Dispose() : Reader.Close()
            If Text.Contains("access_token") Then
                ' get the instagram token and use it to signup
                Dim token As String = RegularExpressions.Regex.Match(Text, """access_token"":""(.*?)""").Groups.Item(1).Value
                If Not ListBox1.Items.Contains(token) Then
                    ListBox1.Items.Add(token)
                End If
            Else
                MsgBox(Text)
            End If
        Catch ex As WebException
            If ex.Message.Contains("500") Then
                MsgBox("Invalid token 
try to get the token again from the same account
or check if the account is disabled
or the token is already exchanged")
            Else
                MsgBox(ex.Message)
            End If
        End Try
    End Sub


    Sub signup(token As String)
        Try : Net.ServicePointManager.CheckCertificateRevocationList = False : Net.ServicePointManager.DefaultConnectionLimit = 300 : Net.ServicePointManager.UseNagleAlgorithm = False : Net.ServicePointManager.Expect100Continue = False : Net.ServicePointManager.SecurityProtocol = 3072
            Dim Encoding As New Text.UTF8Encoding
            Dim username As String = random_string()
            Dim cc As New CookieContainer
            Dim str = ("signed_body=SIGNATURE.{""jazoest"":""22420"",""allow_contacts_sync"":""true"",""dryrun"":""false"",""fb_reg_flag"":""true"",""phone_id"":""#"",""_csrftoken"":""y"",""username"":""" + username + """,""adid"":""#"",""guid"":""#"",""device_id"":""#"",""waterfall_id"":""#"",""fb_access_token"":""" + token + """}").Replace("#", Guid.NewGuid.ToString)
            Dim Bytes As Byte() = Encoding.GetBytes(str)
            Dim AJ As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create("https://i.instagram.com/api/v1/fb/facebook_signup/"), Net.HttpWebRequest)
            If useProxy.CheckState = 1 Then
                AJ.Proxy = random_proxy()
            End If
            With AJ
                .Method = "POST"
                .CookieContainer = cc
                .UserAgent = "Instagram 170.2.0.30.474 Android (25/7.1.2; 240dpi; 720x1280; samsung; SM-N975F; SM-N975F; intel; en_US; 267925697)"
                .Headers.Add("Accept-Language: en-US")
                .Headers.Add("Cookie: mid=y; csrftoken=y; ds_user_id=0")
                .Headers.Add("X-MID: y")
                .Headers.Add("IG-USER-ID: 0")
                .ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
                .AutomaticDecompression = Net.DecompressionMethods.Deflate Or Net.DecompressionMethods.GZip
                .Host = "i.instagram.com"
                .Headers.Add("X-FB-HTTP-Engine: Liger")
                .Headers.Add("X-FB-Client-IP: True")
                .ContentLength = Bytes.Length
            End With
            Dim Stream As IO.Stream = AJ.GetRequestStream() : Stream.Write(Bytes, 0, Bytes.Length) : Stream.Dispose() : Stream.Close()
            Dim postresponse As HttpWebResponse
            postresponse = DirectCast(AJ.GetResponse(), HttpWebResponse)
            Dim Reader As New IO.StreamReader(DirectCast(AJ.GetResponse(), Net.HttpWebResponse).GetResponseStream()) : Dim Text As String = Reader.ReadToEnd : Reader.Dispose() : Reader.Close()
            If Text.Contains("""account_created"":true") Then
                Threading.Thread.Sleep(TimeSpan.FromSeconds(3))
                fbid = RegularExpressions.Regex.Match(Text, """fb_user_id"":""(.*?)"",""").Groups.Item(1).Value
                If Not ListBox2.Items.Contains(username) Then
                    ListBox2.Items.Add(username + " | " + postresponse.Cookies("sessionid").Value)
                    accounts.Append(username + ":" + postresponse.Cookies("sessionid").Value)
                    change_mail(postresponse.Cookies("sessionid").Value)
                End If
            ElseIf Text.Contains("feedback") Or Text.Contains("logged_in") Then
                ' Feedback = account creation limited 
                ' logged_in = couldn't create a new account & stuck with the old one
                ls += 1
                ' skip the token
            End If
        Catch ex As WebException
            If ex.Message.Contains("429") Then
                TextBox2.AppendText("429 - " + ex.Message + vbNewLine)
                signup(token)
            ElseIf ex.Message.Contains("400") Then
                TextBox2.AppendText("400 - " + ex.Message + vbNewLine)
                ls += 1
            Else
                ls += 1
            End If
        End Try
    End Sub


    Sub change_mail(cookie As String)
        Try : Net.ServicePointManager.CheckCertificateRevocationList = False : Net.ServicePointManager.DefaultConnectionLimit = 300 : Net.ServicePointManager.UseNagleAlgorithm = False : Net.ServicePointManager.Expect100Continue = False : Net.ServicePointManager.SecurityProtocol = 3072
            ' change the instagram mail to be able to create more than one account with the facebook account
            Dim Encoding As New Text.UTF8Encoding
            Dim email = random_string()
            Dim str As String = "signed_body=SIGNATURE.{""phone_id"":""#"",""_csrftoken"":""y"",""send_source"":""personal_information"",""_uid"":""0"",""guid"":""#"",""device_id"":""#"",""_uuid"":""#"",""email"":""" + email + "@gmail.com""}".Replace("#", Guid.NewGuid.ToString)
            Dim Bytes As Byte() = Encoding.GetBytes(str)
            Dim AJ As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create("https://i.instagram.com/api/v1/accounts/send_confirm_email/"), Net.HttpWebRequest)
            If useProxy.CheckState = 1 Then
                AJ.Proxy = random_proxy()
            End If
            With AJ
                .Method = "POST"
                .UserAgent = "Instagram 170.2.0.30.474 Android (25/7.1.2; 240dpi; 720x1280; samsung; SM-N975F; SM-N975F; intel; en_US; 267925697)"
                .Headers.Add("Accept-Language: en-US")
                .Headers.Add("Cookie: sessionid=" + cookie + "; mid=y; csrftoken=y; rur=VLL;")
                .Headers.Add("X-MID: YBCBqQABAAE70_bEH9cIBb8eSEbl")
                .Headers.Add("IG-U-DS-USER-ID: " + New Random().Next(0, 99999).ToString)
                .Headers.Add("IG-U-RUR: VLL")
                .Headers.Add("DEBUG-IG-USER-ID: 0")
                .ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
                .AutomaticDecompression = Net.DecompressionMethods.Deflate Or Net.DecompressionMethods.GZip
                .Host = "i.instagram.com"
                .Headers.Add("X-FB-HTTP-Engine: Liger")
                .Headers.Add("X-FB-Client-IP: True")
                .ContentLength = Bytes.Length
            End With
            Dim Stream As IO.Stream = AJ.GetRequestStream() : Stream.Write(Bytes, 0, Bytes.Length) : Stream.Dispose() : Stream.Close()
            Dim Reader As New IO.StreamReader(DirectCast(AJ.GetResponse(), Net.HttpWebResponse).GetResponseStream()) : Dim Text As String = Reader.ReadToEnd : Reader.Dispose() : Reader.Close()
            If Text.Contains("Follow the link") Then
                Threading.Thread.Sleep(TimeSpan.FromSeconds(3))
                accounts.Append(":" + email + "@gmail.com")
                unlink_facebook(cookie)
            End If
        Catch ex As WebException
            MsgBox(ex.Message)
        End Try
    End Sub
    Sub unlink_facebook(cookie As String)
        Try : Net.ServicePointManager.CheckCertificateRevocationList = False : Net.ServicePointManager.DefaultConnectionLimit = 300 : Net.ServicePointManager.UseNagleAlgorithm = False : Net.ServicePointManager.Expect100Continue = False : Net.ServicePointManager.SecurityProtocol = 3072
            ' unlink facebook from the instagram account to be able to create another account with the same facebook account
            Dim Encoding As New Text.UTF8Encoding
            Dim Bytes As Byte() = Encoding.GetBytes($"params=%7B%22server_params%22%3A%7B%22platform%22%3A0%2C%22account_name%22%3A%22%D0%9F%D0%B0%D1%83%D0%BB%D0%B0+%D0%9A%D1%83%D0%BB%D0%B8%D0%BA%D0%BE%D0%B2%D0%B0%22%2C%22flow%22%3A%22ig_im%22%2C%22account_id%22%3A{fbid}%7D%7D&_csrftoken=6ZDfnzZ67hVLceqxcuCLuCdG8qhr9Bjk&_uuid=7a4dcb47-0663-46c8-b37c-aeaa9a67f35b&nest_data_manifest=true&bloks_versioning_id=bfe7510720e920cb359b6fc8e96cfb8323a7127b448ecd0d54dc057e3720e766")
            Dim AJ As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create("https://i.instagram.com/api/v1/bloks/apps/com.bloks.www.fxcal.unlink.async/"), Net.HttpWebRequest)
            If useProxy.CheckState = 1 Then
                AJ.Proxy = random_proxy()
            End If
            With AJ
                .Method = "POST"
                .UserAgent = "Instagram 170.2.0.30.474 Android (25/7.1.2; 240dpi; 720x1280; samsung; SM-N975F; SM-N975F; intel; en_US; 267925697)"
                .Headers.Add("Accept-Language: en-US")
                .Headers.Add("Cookie: sessionid=" + cookie)
                .Headers.Add("X-MID: y")
                .Headers.Add("IG-U-DS-USER-ID: 0")
                .Headers.Add("IG-U-RUR: VLL")
                .Headers.Add("DEBUG-IG-USER-ID: 0")
                .ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
                .AutomaticDecompression = Net.DecompressionMethods.Deflate Or Net.DecompressionMethods.GZip
                .Host = "i.instagram.com"
                .Headers.Add("X-FB-HTTP-Engine: Liger")
                .Headers.Add("X-FB-Client-IP: True")
                .ContentLength = Bytes.Length
            End With
            Dim Stream As IO.Stream = AJ.GetRequestStream() : Stream.Write(Bytes, 0, Bytes.Length) : Stream.Dispose() : Stream.Close()
            Dim Reader As New IO.StreamReader(DirectCast(AJ.GetResponse(), Net.HttpWebResponse).GetResponseStream()) : Dim Text As String = Reader.ReadToEnd : Reader.Dispose() : Reader.Close()
            Threading.Thread.Sleep(TimeSpan.FromSeconds(1.9))
            change_pass(cookie)
        Catch ex As WebException : Dim AJJ As String = New IO.StreamReader(ex.Response.GetResponseStream()).ReadToEnd() : MsgBox("Error : " & AJJ) : End Try
    End Sub


    Sub change_pass(session As String)
        Try : Net.ServicePointManager.CheckCertificateRevocationList = False : Net.ServicePointManager.DefaultConnectionLimit = 300 : Net.ServicePointManager.UseNagleAlgorithm = False : Net.ServicePointManager.Expect100Continue = False : Net.ServicePointManager.SecurityProtocol = 3072
            Dim password = random_string()
            Dim Encoding As New Text.UTF8Encoding
            Dim Bytes As Byte() = Encoding.GetBytes("signed_body=SIGNATURE.{""_csrftoken"":""2TqtFkzC64u8szz7C7n8v0wtz7mkYnI2"",""_uid"":""0"",""_uuid"":""" + Guid.NewGuid.ToString + """,""enc_new_password"":""#PWD_INSTAGRAM:0:0:" + password + """}")
            Dim AJ As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create("https://i.instagram.com/api/v1/accounts/change_password/"), Net.HttpWebRequest)
            If useProxy.CheckState = 1 Then
                AJ.Proxy = random_proxy()
            End If
            With AJ
                .Method = "POST"
                .UserAgent = "Instagram 170.2.0.30.474 Android (25/7.1.2; 240dpi; 720x1280; samsung; SM-N975F; SM-N975F; intel; en_US; 267925697)"
                .Headers.Add("Accept-Language: en-US")
                .Headers.Add($"Cookie: sessionid={session}")
                .Headers.Add("X-MID: y")
                .Headers.Add("IG-U-DS-USER-ID: 0")
                .Headers.Add("IG-U-RUR: FRC")
                .Headers.Add("DEBUG-IG-USER-ID: 0")
                .ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
                .AutomaticDecompression = Net.DecompressionMethods.Deflate Or Net.DecompressionMethods.GZip
                .Host = "i.instagram.com"
                .Headers.Add("X-FB-HTTP-Engine: Liger")
                .Headers.Add("X-FB-Client-IP: True")
                .ContentLength = Bytes.Length
            End With
            Dim Stream As IO.Stream = AJ.GetRequestStream() : Stream.Write(Bytes, 0, Bytes.Length) : Stream.Dispose() : Stream.Close()
            Dim Reader As New IO.StreamReader(DirectCast(AJ.GetResponse(), Net.HttpWebResponse).GetResponseStream()) : Dim Text As String = Reader.ReadToEnd : Reader.Dispose() : Reader.Close()
            If Text.Contains("ok") Then
                accounts.Append(":" + password + vbNewLine)
                Threading.Thread.Sleep(TimeSpan.FromSeconds(3))
            End If
        Catch ex As WebException : Dim AJJ As String = New IO.StreamReader(ex.Response.GetResponseStream()).ReadToEnd() : MsgBox("Error : " & AJJ) : End Try
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label1.Text = "Total Made: " + ListBox2.Items.Count.ToString
        TextBox1.Text = accounts.ToString
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim str As New StringBuilder
            For Each item As String In ListBox1.Items
                str.AppendLine(item)
            Next
            Dim rndpath As String = desktop + $"\{New Random().Next(0, 99999).ToString}_tokens.txt"
            IO.File.WriteAllText(desktop + $"\{New Random().Next(0, 99999).ToString}_tokens.txt", str.ToString)
            MsgBox("Exported to " + rndpath)
            Process.Start(rndpath)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        MsgBox("2 Step Code button - Import the codes that you got from the Token extractor
Import button - Import already exchanged codes")
    End Sub
End Class
