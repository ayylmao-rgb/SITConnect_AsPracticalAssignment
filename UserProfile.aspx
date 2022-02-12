<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserProfile.aspx.cs" Inherits="SITConnect_AsPracticalAssignment.UserProfile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <h1>You have successfully Logged In!</h1>
    <br />
    <br />
    <h4>My Profile</h4>
    <div>
        <div class="row">
            <p>
                <asp:Image ID="profilepicturehere" Height="100" Width="100" runat="server" />

            </p>
        </div>
        <div class="row">
            <p>
                <asp:Label Font-Bold="true" ID="fnamelabel" class="mr-4" runat="server" Text="First Name"></asp:Label>
                <asp:Label ID="firstnamehere" runat="server"></asp:Label>
            </p>
        </div>
        <div class="row">
            <p>
                <asp:Label Font-Bold="true" ID="lnamelabel" class="mr-4" runat="server" Text="Last Name"></asp:Label>
                <asp:Label ID="lastnamehere" runat="server"></asp:Label>
            </p>
        </div>
        <div class="row">
            <p>
                <asp:Label Font-Bold="true" ID="creditcardlabel" class="mr-4" runat="server" Text="Credit Card Number"></asp:Label>
                <asp:Label ID="creditnumberhere" runat="server"></asp:Label>
            </p>
        </div>
        <div class="row">
            <p>
                <asp:Label Font-Bold="true" ID="emailaddresslabel" class="mr-4" runat="server" Text="Your Email"></asp:Label>
                <asp:Label ID="emailhere" runat="server"></asp:Label>
            </p>
        </div>
    <div class="row">
        <p>
            <asp:Label Font-Bold="true" ID="doblabel" class="mr-4" runat="server" Text="date of birth"></asp:Label>
            <asp:Label ID="dobhere" runat="server"></asp:Label>
        </p>
    </div>
    <form id="form1" runat="server">
        <asp:Button ID="logoutbtn" runat="server" Text="Logout" OnClick="Logout" />

    </form>
    <a href="enable2fa.aspx" class="btn" type="button" height="50px" width="100px">Enable 2FA</a>
    <a href="PasswordChange.aspx" type="button" class="btn" height="50px" width="100px">change my password</a>
    <a href="buystationary.aspx" type="button" class="btn" height="50px" width="100px">View Store</a>



    </div>
</body>
</html>
