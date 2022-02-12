using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Google.Authenticator;


namespace SITConnect_AsPracticalAssignment
{
    public partial class _2FAverify : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }
        public Boolean ValidateTwoFactorPIN(String pin)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var result = userAuthenticationCode();
            if (result != "error")
            {
                return tfa.ValidateTwoFactorPIN(result, pin);

            }
            else
            {
                return false;
            }
        }
        protected void btnValidate_Click(object sender, EventArgs e)
        {
            String pin = txtSecurityCode.Text.Trim();
            Boolean status = ValidateTwoFactorPIN(pin);
            if (status)
            {
                Response.Redirect("UserProfile.aspx", false);
            }
            else
            {
                lblResult.Visible = true;
                lblResult.Text = "Invalid Code. Please try again";
            }
        }
        protected string userAuthenticationCode()
        {
            var result = "";
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Account WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", (string)Session["UserID"]);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["AuthenticationCode"] != DBNull.Value)
                        {
                            result = reader["AuthenticationCode"].ToString();
                        }
                    }
                }
            }//try
            catch (Exception ex)
            {
                result = "error";
                throw new Exception(ex.ToString());

            }
            finally
            {
                connection.Close();
            }
            return result;
        }
    }
}