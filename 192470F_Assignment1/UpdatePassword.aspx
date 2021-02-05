<%@ Page Title="Update Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UpdatePassword.aspx.cs" Inherits="_192470F_Assignment1.UpdatePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />

    <asp:Label ID="Result1" runat="server"></asp:Label>

    <br />
    Old Password:
    <asp:TextBox ID="old_pwd" runat="server" TextMode="Password"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
        ControlToValidate="old_pwd"
        ErrorMessage="Password is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    <br />

    <asp:Label ID="old_pwd_check" runat="server"></asp:Label>

    <br />
    <br />
    New Password:
    <asp:TextBox ID="new_pwd" runat="server" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>

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

    <br />
    <br />
    <asp:Button ID="btn_update" runat="server" Text="Update" Width="186px" OnClick="btn_update_Click" />

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
