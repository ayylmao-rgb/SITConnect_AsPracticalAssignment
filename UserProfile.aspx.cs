using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SITConnect_AsPracticalAssignment
{
    public partial class UserProfile : System.Web.UI.Page
    {
        // declare variables
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] CreditCardNumber = null;
        string emaillogin = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            // check if there is a session if not user will not be able to access this page
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    // display error messages
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    // display user's relevant details
                    emaillogin = (string)Session["userID"];
                    displayProfile(emaillogin);
                }
            }
            else
            {
                // display error messages
                Response.Redirect("Login.aspx", false);
            }
        }

        // if user wishes to log out
        protected void Logout(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                if (Request.Cookies["AuthToken"] != null)
                {
                    Response.Cookies["AuthToken"].Value = string.Empty;
                    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                }

            }
        }

        // function to display relevant details
        protected void displayProfile(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Account WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Fname"] != DBNull.Value)
                        {
                            firstnamehere.Text = reader["Fname"].ToString();
                        }
                        if (reader["Lname"] != DBNull.Value)
                        {
                            lastnamehere.Text = reader["Lname"].ToString();
                        }
                        if (reader["CreditNo"] != DBNull.Value)
                        {
                            CreditCardNumber = Convert.FromBase64String(reader["CreditNo"].ToString());
                        }
                        if (reader["EmailAddress"] != DBNull.Value)
                        {
                            emailhere.Text = reader["EmailAddress"].ToString();
                        }
                        if (reader["DateOfBirth"] != DBNull.Value)
                        {
                            dobhere.Text = reader["DateOfBirth"].ToString();
                        }
                        if (reader["ProfilePath"] != DBNull.Value)
                        {
                            profilepicturehere.Attributes["src"] = reader["ProfilePath"].ToString();
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                    }
                    creditnumberhere.Text = decryptData(CreditCardNumber);
                }
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        // decrypt email in order to display the correct credit card number
        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecryot = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string
                            plainText = srDecryot.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { }
            return plainText;
        }
    }
}