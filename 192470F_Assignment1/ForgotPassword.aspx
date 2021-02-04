<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="_192470F_Assignment1.ForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />

    <label for="id">Email</label>
    <br />
    <asp:TextBox ID="Email" runat="server"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
        ControlToValidate="Email"
        ErrorMessage="Username is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    <br />
    <asp:Label ID="EmailCheck" runat="server"></asp:Label>

    <br />
    <br />

    <div id="id1" runat="server">
        <label for="ans">What is your nickname? (Used for security reason - Reset Password)</label>
        <br />
        <asp:TextBox ID="Ans" runat="server"></asp:TextBox>

        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
            ControlToValidate="Ans"
            ErrorMessage="Security answer is a required field."
            ForeColor="Red">
        </asp:RequiredFieldValidator>

        <br />
        <asp:Label ID="SecurityAnsCheck" runat="server"></asp:Label>

        <br />
        <br />
        New Password:
    <asp:TextBox ID="new_pwd" runat="server" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>

        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
            ControlToValidate="new_pwd"
            ErrorMessage="Password is a required field."
            ForeColor="Red">
        </asp:RequiredFieldValidator>

        <br />

        <asp:Label ID="IbI_pwdchecker2" runat="server"></asp:Label>
        <p id="IbI_pwdchecker"></p>

        Confirm Password:
    <asp:TextBox ID="cfm_pwd" runat="server" TextMode="Password"></asp:TextBox>

        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
            ControlToValidate="cfm_pwd"
            ErrorMessage="Password is a required field."
            ForeColor="Red">
        </asp:RequiredFieldValidator>

        <br />

        <asp:Label ID="passwordValidate" runat="server"></asp:Label>

    </div>

    <br />
    <br />
    <asp:Button ID="btn_reset" runat="server" Text="Reset" Width="186px" OnClick="btn_reset_Click" />

    <br />
    <br />
    <asp:Label ID="lblMessage" runat="server"></asp:Label>


    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=new_pwd.ClientID %>').value;

            if (str.length < 8) {
                document.getElementById("IbI_pwdchecker").innerHTML = "Password Length must be at least 8 characters";
                document.getElementById("IbI_pwdchecker").style.color = "Red";
                return ("too_short");
            }

            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("IbI_pwdchecker").innerHTML = "Password require at least 1 number";
                document.getElementById("IbI_pwdchecker").style.color = "Red";
                return ("no_number");
            }

            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("IbI_pwdchecker").innerHTML = "Password must contain at least 1 uppercase character";
                document.getElementById("IbI_pwdchecker").style.color = "Red";
                return ("no_uppercase");
            }

            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("IbI_pwdchecker").innerHTML = "Password must contain at least 1 lowercase character";
                document.getElementById("IbI_pwdchecker").style.color = "Red";
                return ("no_lowercase");
            }

            else if (str.search(/[^A-Za-z0-9]/) == -1) {
                document.getElementById("IbI_pwdchecker").innerHTML = "Password must contain at least 1 special character";
                document.getElementById("IbI_pwdchecker").style.color = "Red";
                return ("no_special_character");
            }

            document.getElementById("IbI_pwdchecker").innerHTML = "Excellent";
            document.getElementById("IbI_pwdchecker").style.color = "Blue";
        }
    </script>
</asp:Content>
