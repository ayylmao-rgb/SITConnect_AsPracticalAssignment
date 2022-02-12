﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Enable2FA.aspx.cs" Inherits="SITConnect_AsPracticalAssignment.enable2fa" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="container">
                <div class="jumbotron">
                    <h2 class="text-info text-center">Enable google authentication</h2>
                    <hr />
                    <div class="row">
                        <div class="col-md-12">
                            <div class="section">
                                <h3 class="text-info">Step 1: Install Google Authenticator  
                                </h3>
                                <p>Please download and install Google Authenticator on your IPhone/IPad/Android device, if already not installed.</p>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="section">
                                <h3 class="text-info">Step 2: Link your device to your account:  
                                </h3>
                                <p>You have two options to link your device to your account:</p>
                                <p><b>Using QR Code:</b> Select<b> Scan a barcode</b>. If the Authenticator app cannot locate a barcode scanner app on your mobile device, you might be prompted to download and install one. If you want to install a barcode scanner app so you can complete the setup process, select Install, then go through the installation process. Once the app is installed, reopen Google Authenticator, then point your camera at the QR code on your computer screen.</p>

                                <p>
                                    <b>Using Secret Key:</b>
                                    Select <b>Enter provided key</b>, then enter account name of your account in the <b>"Enter account name"</b> box. Next, enter the secret key appear on your computer screen in the <b>"Enter your key"</b> box. Make sure you've chosen to make the key Time based, then select Add.  
                                </p>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="col-md-4">
                                <asp:Image ID="qrbarcode" runat="server" Style="height: 300px; width: 300px;" />
                            </div>
                            <div class="col-md-6">
                                <div>
                                    <span style="font-weight: bold; font-size: 14px;">Your Account Name:</span>
                                </div>
                                <div>
                                    <asp:Label runat="server" ID="acclbl"></asp:Label>
                                </div>

                                <div>
                                    <span style="font-weight: bold; font-size: 14px;">Secret Key:</span>
                                </div>
                                <div>
                                    <asp:Label runat="server" ID="codesetupmanual"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="clearfix"></div>
                        <div class="col-md-12" style="margin-top: 2%">
                            <div class="form-group col-md-4">
                                <asp:TextBox runat="server" CssClass="form-control" ID="txtverifycode" MaxLength="50" ToolTip="Please enter security code you get on your authenticator application">  
                                </asp:TextBox>
                            </div>
                            <asp:Button ID="btnValidate" OnClick="submitbutton" CssClass="btn btn-primary" runat="server" Text="Validate" />
                        </div>
                        <div class="alert alert-success col-md-12" runat="server" role="alert">
                            <asp:Label ID="resultshere" runat="server" Text=""></asp:Label>
                        </div>

                                <a href="userProfile.aspx" class="btn" type="button" height="50px" width="100px">go back</a>

                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
