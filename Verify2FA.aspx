<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Verify2FA.aspx.cs" Inherits="SITConnect_AsPracticalAssignment._2FAverify" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
             <h1>Google Authentication. Please enter your verification Code</h1>
                <div class="col-md-12" style="margin-top: 2%">
                        <div class="form-group col-md-4">
                            <asp:TextBox runat="server" Width="300px" CssClass="form-control" ID="txtSecurityCode" MaxLength="50" ToolTip="Please enter security code you get on your authenticator application">  
                            </asp:TextBox>
                        </div>
                        <asp:Button ID="btnValidate" OnClick="btnValidate_Click" CssClass="btn btn-primary" runat="server" Text="Validate" />
                    </div>
                    <div class="alert alert-success col-md-12" runat="server" role="alert">
                        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                    </div>
        </div>
    </form>
</body>
</html>
