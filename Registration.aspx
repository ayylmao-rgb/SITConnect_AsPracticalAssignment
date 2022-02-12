<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect_AsPracticalAssignment.registration" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="//code.jquery.com/jquery-1.11.2.min.js" type="text/javascript"></script>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous">
    <script src="https://www.google.com/recaptcha/api.js?render=6Leu_VweAAAAABaEdi7Pci-pHj_jWPX6PvWRG3Au"></script>
    <script type="text/javascript">
        function ShowImagePreview(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#<%=imgpreview.ClientID%>').prop('src', e.target.result)
                        .width(240)
                        .height(150);
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;

            if (str.length < 12) {
                document.getElementById("passworderror").innerHTML = "Password Length Must be at least 12 characters";
                document.getElementById("passworderror").style.color = "Red";
                return ("too_short");
            }

            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("passworderror").innerHTML = "Password require at least 1 number";
                document.getElementById("passworderror").style.color = "Red";
                return ("no_number");
            }

            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("passworderror").innerHTML = "Password require at least Upper cases";
                document.getElementById("passworderror").style.color = "Red";
                return ("no_upper_case");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("passworderror").innerHTML = "Password require at least lower cases";
                document.getElementById("passworderror").style.color = "Red";
                return ("no_lower_case");
            }
            else if (str.search(/[!@#$%^&*]/) == -1) {
                document.getElementById("passworderror").innerHTML = "Password require at least special characters";
                document.getElementById("passworderror").style.color = "Red";
                return ("no_special_characters");
            }

            document.getElementById("passworderror").innerHTML = "Excellent!"
            document.getElementById("passworderror").style.color = "Blue";
        }
    </script>
</head>
<body>
    <h1>Registration</h1>
    <form id="form1" runat="server">
        <div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="FirstNameLabel" class="mr-4" runat="server" Text="First Name"></asp:Label>
                    <asp:TextBox required="true" CssClass="ml-4" ID="firstname" placeholder="what is your first name?" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="firstnameerror" runat="server"></asp:Label>
                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="LastNameLabel" class="mr-4" runat="server" Text="Last Name"></asp:Label>
                    <asp:TextBox required="true" CssClass="ml-4" ID="lastname" placeholder="what is your last name?" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="lastnameerror" runat="server"></asp:Label>
                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="creditcardnumberLabel" class="mr-4" runat="server" Text="Credit card number"></asp:Label>
                    <asp:TextBox required="true" MaxLength="16" TextMode="Number" CssClass="ml-4" ID="creditcard" placeholder="1234567890123456" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="creditcarderror" runat="server"></asp:Label>
                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="emailaddressLabel" class="mr-4" runat="server" Text="Email"></asp:Label>
                    <asp:TextBox required="true" TextMode="Email" CssClass="ml-4" ID="email" placeholder="mymail@gmail.com" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="emailerror" runat="server"></asp:Label>
                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="passwordLabel" class="mr-4" runat="server" Text="Password"></asp:Label>
                    <asp:TextBox required="true" onkeyup="javascript:validate()" TextMode="Password" CssClass="ml-4" ID="tb_password" placeholder="Your password" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="passworderror" runat="server"></asp:Label>
                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="dobLabel" class="mr-4" runat="server" Text="Date of Birth"></asp:Label>
                    <asp:TextBox required="true" TextMode="Date" CssClass="ml-4" ID="dateofbirth" placeholder="When's your birthday?" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="doberror" runat="server"></asp:Label>
                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:Label ID="profilepictureLabel" class="mr-4" runat="server" Text="Upload profile picture"></asp:Label>


                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:Image ID="imgpreview" Height="150px" Width="240px" runat="server" /><br />

                </p>
            </div>
            <div class="form-group row">
                <p>
                    <asp:FileUpload ID="PhotoUpload" runat="server" onchange="ShowImagePreview(this);" />
                    <asp:Label ID="PhotoUploadLabel" runat="server"></asp:Label>
                    <asp:Label ID="pictureerror" runat="server"></asp:Label>
                </p>
            </div>

        </div>
        <p>
            <asp:Button ID="Button1" OnClick="ButtonSubmit_Click" Text="Sign up" CssClass="btn btn-primary" Height="50px" Width="200px" runat="server" />

        </p>
        <div class="form-group mb-4">
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
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
<!-- 
    TO DO List





Account lockout after 3 login failures.
lean logout
Perform audit log

Prevent SQLi and perform proper inputfiltering, validation and verification. (e.g email)
Client and server input validation


Provide test cases in your report.

Use external tools to perform software testing: Github (check week 14 eLab)
Implement the recommendation to clear the ecurity vulnerability for your source code.
Save your source code into Github and provide the public link

Automatic account recovery after x mins of lockout.
Avoid password reuse (max 2 password history)
Minimum and Maximum password age (cannot change password within x mins from the last change of password and must change password after x mins)




    ->
