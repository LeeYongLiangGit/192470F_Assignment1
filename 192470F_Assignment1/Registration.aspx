<%@ Page Title="Registration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="_192470F_Assignment1.Registration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="title">Registration Form</h2>

    <asp:Label ID="Result" runat="server"></asp:Label>

    <br />

    <label for="firstname">First Name</label>
    <br />
    <asp:TextBox ID="firstName" runat="server"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
        ControlToValidate="firstname"
        ErrorMessage="First Name is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    <br />
    <br />

    <label for="lastname">Last Name</label>
    <br />
    <asp:TextBox ID="lastName" runat="server"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
        ControlToValidate="lastname"
        ErrorMessage="Last Name is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    <br />
    <br />

    <label for="cc">Credit Card Number</label>
    <br />
    <asp:TextBox ID="CardNum" runat="server"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
        ControlToValidate="CardNum"
        ErrorMessage="Credit Card Number is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    <br />
    <br />

    <label for="exDate">Expiration Date</label>
    <br />
    <asp:TextBox ID="ValidDate" runat="server" TextMode="Date"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
        ControlToValidate="ValidDate"
        ErrorMessage="Expiration Date is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    <br />
    <br />

    <label for="email">Email</label>
    <br />
    <asp:TextBox ID="email" runat="server" TextMode="Email"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
        ControlToValidate="email"
        ErrorMessage="Email is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    &nbsp;

    <br />

    <asp:Label ID="emailValidate" runat="server"></asp:Label>

    <br />
    <br />

    <label for="Password">Password</label>
    <br />
    <asp:TextBox ID="tb_password" runat="server" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
        ControlToValidate="tb_password"
        ErrorMessage="Password is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    &nbsp;

    <br />

    <asp:Label ID="IbI_pwdchecker2" runat="server"></asp:Label>
    <p id="IbI_pwdchecker"></p>

    <label for="cfmPassword">Confirm Password</label>
    <br />
    <asp:TextBox ID="cfmpassword" runat="server" TextMode="Password"></asp:TextBox>
    <asp:Label ID="passwordValidate" runat="server"></asp:Label>

    <br />
    <br />

    <label for="dob">Date Of Birth</label>
    <br />
    <asp:TextBox ID="dob" runat="server" TextMode="Date"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
        ControlToValidate="dob"
        ErrorMessage="BirthDate is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    <br />
    <br />

    <label for="ans">What is your nickname? (Used for security reason - Reset Password)</label>
    <br />
    <asp:TextBox ID="Ans" runat="server"></asp:TextBox>

    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
        ControlToValidate="Ans"
        ErrorMessage="Security answer is a required field."
        ForeColor="Red">
    </asp:RequiredFieldValidator>

    <br />
    <br />

    <asp:Button ID="btn_Submit" runat="server" OnClick="btn_Submit_Click" Text="Submit" />

    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;

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
