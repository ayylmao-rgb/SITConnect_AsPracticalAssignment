<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect_AsPracticalAssignment.Login" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous">
    <script src="https://www.google.com/recaptcha/api.js?render=6Leu_VweAAAAABaEdi7Pci-pHj_jWPX6PvWRG3Au"></script>
    
</head>
<body>
    <h1>Login</h1>
    <form id="form1" runat="server">
        <div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="EmailLabel" class="mr-4" runat="server" Text="Email"></asp:Label>
                    <asp:TextBox required="true" CssClass="ml-4" ID="emailaddress" placeholder="youremail@gmail.com" TextMode="Email" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="emailaddresserror" runat="server"></asp:Label>
                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="PasswordLabel" class="mr-4" runat="server" Text="Password"></asp:Label>
                    <asp:TextBox required="true" TextMode="Password" CssClass="ml-4" ID="password" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="lastnameerror" runat="server"></asp:Label>
                </p>
            </div>
            <p>
            <asp:Button ID="Button1" OnClick="ButtonSubmit_Click" Text="LogIn" CssClass="btn btn-primary" Height="50px" Width="200px" runat="server" />
                 <div class="form-group mb-4">
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </div>
        </p>
        </div>
    </form>
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

