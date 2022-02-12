using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Google.Authenticator;

namespace SITConnect_AsPracticalAssignment
{
    public partial class enable2fa : System.Web.UI.Page
    {
        //google authentication declaring variables
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        String CodeAuthent
        {
            get
            {
                if (ViewState["CodeAuthent"] != null)
                    return ViewState["CodeAuthent"].ToString().Trim();
                return String.Empty;
            }
            set
            {
                ViewState["CodeAuthent"] = value.Trim();
            }
        }

        String titleAuthent
        {
            get
            {
                return (string)Session["UserID"];
            }
        }


        String barcodeimgAuth
        {
            get;
            set;
        }

        String manualcodeauth
        {
            get;
            set;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    if (!Page.IsPostBack)
                    {
                        resultshere.Text = String.Empty;
                        resultshere.Visible = false;
                        GenerateTwoFactorAuthentication();
                        qrbarcode.ImageUrl = barcodeimgAuth;
                        codesetupmanual.Text = manualcodeauth;
                        acclbl.Text = titleAuthent;
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

        }

        protected void submitbutton(object sender, EventArgs e)
        {
            String pin = txtverifycode.Text.Trim();
            Boolean status = ValidateTwoFactorPIN(pin);
            if (status)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE Account SET [Authentication]=@enableauth,[AuthenticationCode]=@authcode WHERE EmailAddress='" + (string)Session["UserID"] + "'"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                try
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@enableauth", "true");
                                    cmd.Parameters.AddWithValue("@authcode", CodeAuthent);
                                    cmd.Connection = con;
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                                catch (SqlException ex)
                                {
                                    resultshere.Text = ex.ToString();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                resultshere.Visible = true;
                resultshere.Text = "Code Successfully Verified. Google Authentication is enabled.";
            }
            else
            {
                resultshere.Visible = true;
                resultshere.Text = "Invalid Code. Please try again";
            }
        }

        public Boolean ValidateTwoFactorPIN(String pin)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            return tfa.ValidateTwoFactorPIN(CodeAuthent, pin);
        }

        public Boolean GenerateTwoFactorAuthentication()
        {
            Guid guid = Guid.NewGuid();
            String uniqueUserKey = Convert.ToString(guid).Replace("-", "").Substring(0, 10);
            CodeAuthent = uniqueUserKey;

            Dictionary<String, String> result = new Dictionary<String, String>();
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("SITConnect", titleAuthent, CodeAuthent, false, 300);
            if (setupInfo != null)
            {
                barcodeimgAuth = setupInfo.QrCodeSetupImageUrl;
                manualcodeauth = setupInfo.ManualEntryKey;
                return true;
            }
            return false;
        }
    }
}