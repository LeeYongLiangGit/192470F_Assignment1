<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="_192470F_Assignment1.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="https://www.google.com/recaptcha/api.js?render="></script>

    <table>
        <tr>
            <td colspan="2">
                <h3>Login</h3>
                <asp:Label ID="Result" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>Username (Email):
                        </td>
                        <td>
                            <asp:TextBox ID="tb_email" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Password:</td>
                        <td>
                            <asp:TextBox ID="tb_pwd" runat="server" TextMode="Password"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <ul>
                    <li><a href="ForgotPassword.aspx">Forgot your password?</a></li>
                </ul>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btn_login" runat="server" OnClick="btn_login_Click" Text="Login" Width="186px" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
    </table>


    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>

</asp:Content>
