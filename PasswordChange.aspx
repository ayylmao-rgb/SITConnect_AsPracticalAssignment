<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PasswordChange.aspx.cs" Inherits="SITConnect_AsPracticalAssignment.PasswordChange" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="//code.jquery.com/jquery-1.11.2.min.js" type="text/javascript"></script>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous">

    <script src="https://www.google.com/recaptcha/api.js?render=6Leu_VweAAAAABaEdi7Pci-pHj_jWPX6PvWRG3Au"></script>

    <script>
        function validate() {
            var str = document.getElementById('<%=newpassword.ClientID %>').value;

            if (str.length < 12) {
                document.getElementById("pwdchecker").innerHTML = "Password Length Must be at least 12 characters";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("too_short");
            }

            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("pwdchecker").innerHTML = "Password require at least 1 number";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("no_number");
            }

            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("pwdchecker").innerHTML = "Password require at least Upper cases";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("no_upper_case");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("pwdchecker").innerHTML = "Password require at least lower cases";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("no_lower_case");
            }
            else if (str.search(/[!@#$%^&*]/) == -1) {
                document.getElementById("pwdchecker").innerHTML = "Password require at least special characters";
                document.getElementById("pwdchecker").style.color = "Red";
                return ("no_special_characters");
            }

            document.getElementById("pwdchecker").innerHTML = "Excellent!"
            document.getElementById("pwdchecker").style.color = "Blue";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="currentpasswdlabel" runat="server" Text="Current password" Width="120px"
                Font-Bold="True"></asp:Label>
            <asp:TextBox ID="currentpassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                ControlToValidate="currentpassword"
                ErrorMessage="Please enter Current password"></asp:RequiredFieldValidator>
            <br />
            <asp:Label ID="newpasswdlabel" runat="server" Text="New password" Width="120px"
                Font-Bold="True"></asp:Label>

            <asp:TextBox ID="newpassword" runat="server" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
            <asp:Label ID="pwdchecker" runat="server"></asp:Label>

            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                ControlToValidate="newpassword" ErrorMessage="Please enter New password"></asp:RequiredFieldValidator>
            <br />

            <asp:Label ID="confirmpasswordlabel" runat="server" Text="Confirm password" Width="120px"
                Font-Bold="True"></asp:Label>

            <asp:TextBox ID="confirmpassword" runat="server" TextMode="Password"></asp:TextBox>

            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                ControlToValidate="confirmpassword"
                ErrorMessage="Please enter Confirm  password"></asp:RequiredFieldValidator>

            <asp:CompareValidator ID="CompareValidator1" runat="server"
                ControlToCompare="newpassword" ControlToValidate="confirmpassword"
                ErrorMessage="Password does not match"></asp:CompareValidator>
        </div>
        <asp:Button ID="changepwdbutton" runat="server" Font-Bold="True" BackColor="#CCFF99" OnClick="changebtn" Text="change password now" />
        <asp:Label ID="lbl_msg" Font-Bold="True" BackColor="#FFFF66" ForeColor="#FF3300" runat="server" Text=""></asp:Label><br />


        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </form>
    <a href="userProfile.aspx" class="btn" type="button" height="50px" width="100px">go back</a>

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6Leu_VweAAAAABaEdi7Pci-pHj_jWPX6PvWRG3Au', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });

    </script>
</body>
</html>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p" crossorigin="anonymous"></script>
