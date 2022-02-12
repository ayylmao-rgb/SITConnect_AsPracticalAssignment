using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_AsPracticalAssignment
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // logging into the account
        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                if (getnofailattempts())
                {
                    System.Diagnostics.Debug.WriteLine("weeeeeeeeee");

                    var accountfaileddate = getlockouttime();
                    if (accountfaileddate == DateTime.Now)
                    {
                        Response.Redirect("404.aspx", false);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("hallooo");

                        // 50 seconds for testing purposes
                        var recoveryTime = accountfaileddate.AddSeconds(50);
                        if (recoveryTime > DateTime.Now)
                        {
                            lblMessage.Text = "Your Account is locked after 3 failed attempts. Account is lock for 50 seconds ";
                        }
                        else
                        {
                            refreshattempts();
                            string useremail = HttpUtility.HtmlEncode(emailaddress.Text.ToString().Trim());
                            string userpwd = HttpUtility.HtmlEncode(password.Text.ToString().Trim());
                            SHA512Managed hashing = new SHA512Managed();
                            string dbHash = getDBHash(useremail);
                            string dbSalt = getDBSalt(useremail);
                            try
                            {
                                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                                {
                                    string pwdWithSalt = userpwd + dbSalt;
                                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                    string userHash = Convert.ToBase64String(hashWithSalt);
                                    if (userHash.Equals(dbHash))
                                    {
                                        Session["UserID"] = useremail;

                                        // create a new GUID and save into the session
                                        string guid = Guid.NewGuid().ToString();
                                        Session["AuthToken"] = guid;

                                        // check if user has 2fa enabled
                                        var authresult = checkAuthentication(useremail);

                                        // now create a new cookie with this guid value
                                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                        if (authresult == "true")
                                        {
                                            Response.Redirect("Verify2FA.aspx", false);
                                        }
                                        else if (authresult == "false")
                                        {
                                            Response.Redirect("UserProfile.aspx", false);

                                        }
                                        else
                                        {
                                            // unkown error occured

                                            Session.Clear();
                                            Session.Abandon();
                                            Session.RemoveAll();

                                            Response.Redirect("errorpage.aspx", false);

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

                                    }
                                    else
                                    {
                                        lblMessage.Text = "Invalid email/password login details. Please try to login again.";
                                        lblMessage.ForeColor = Color.Red;
                                        if (!getnofailattempts())
                                        {
                                            counterfail();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.ToString());
                            }
                            finally { }
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("straight here");

                    string useremail = HttpUtility.HtmlEncode(emailaddress.Text.ToString().Trim());
                    string userpwd = HttpUtility.HtmlEncode(password.Text.ToString().Trim());
                    SHA512Managed hashing = new SHA512Managed();
                    string dbHash = getDBHash(useremail);
                    string dbSalt = getDBSalt(useremail);
                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = userpwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (userHash.Equals(dbHash))
                            {
                                Session["UserID"] = useremail;

                                // create a new GUID and save into the session
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                // check if user has 2fa enabled
                                var authresult = checkAuthentication(useremail);

                                // now create a new cookie with this guid value
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                if (authresult == "true")
                                {
                                    Response.Redirect("Verify2FA.aspx", false);
                                }
                                else if (authresult == "false")
                                {
                                    Response.Redirect("UserProfile.aspx", false);

                                }
                                else
                                {
                                    // unkown error occured

                                    Session.Clear();
                                    Session.Abandon();
                                    Session.RemoveAll();

                                    Response.Redirect("errorpage.aspx", false);

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

                            }
                            else
                            {
                                lblMessage.Text = "Invalid email/password login details. Please try  to log in again.";
                                lblMessage.ForeColor = Color.Red;
                                if (!getnofailattempts())
                                {
                                    counterfail();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally { }
                }

            }
            else
            {
                Response.Redirect("errorpage.aspx", false);
            }

        }

        //validation of captcha
        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter. 
            //captchaResponse consist of the user click pattern. Behaviour analytics! AI :) 
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
           (" https://www.google.com/recaptcha/api/siteverify?secret=6Leu_VweAAAAAKX7EOG187lbYH_jAvg_mo3jQlSi &response=" + captchaResponse);


            try
            {

                //Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //To show the JSON response string for learning purpose
                        //lbl_gScore.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonObject to handle the response e.g success or Error
                        //Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);//

                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        // getting password hash
        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        // getting password salt
        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        // check if user enabled their two factor authentication
        protected string checkAuthentication(string userid)
        {
            var resultString = "";
            SqlConnection connectionString = new SqlConnection(MYDBConnectionString);
            string sql = "select * FROM Account WHERE EmailAddress=@USERID";
            SqlCommand command = new SqlCommand(sql, connectionString);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connectionString.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Authentication"] != DBNull.Value)
                        {
                            string authtf = reader["Authentication"].ToString();
                            if (authtf == "true")
                            {
                                // if enable, returns true
                                resultString = "true";
                            }
                            else
                            {
                                // user did not enable 2fa
                                resultString = "false";
                            }
                        }
                        else
                        {
                            // user did not enable 2fa
                            resultString = "false";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultString = "error";
                throw new Exception(ex.ToString());

            }
            finally
            {
                connectionString.Close();
            }
            return resultString;
        }


        protected bool getnofailattempts()
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select FailedAttempts FROM Account WHERE EmailAddress='" + HttpUtility.HtmlEncode(emailaddress.Text.ToString().Trim()) + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            System.Diagnostics.Debug.WriteLine("step 1");

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("step 2");

                        if (reader["FailedAttempts"] != DBNull.Value)
                        {
                            if ((reader["FailedAttempts"] != null) && (Convert.ToInt32(reader["FailedAttempts"]) != 3))
                            {
                                System.Diagnostics.Debug.WriteLine("account lockout still below 3");
                                return false;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("account lockout exist");

                                return true;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("step 3");

                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return false;
        }

        protected DateTime getlockouttime()
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LockoutTime FROM Account WHERE EmailAddress='" + HttpUtility.HtmlEncode(emailaddress.Text.ToString().Trim()) + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            DateTime lockoutdate = DateTime.Now;
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LockoutTime"] != DBNull.Value)
                        {
                            lockoutdate = (DateTime)reader["LockoutTime"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return lockoutdate;
        }

        protected void refreshattempts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Update Account set FailedAttempts = @fa, LockoutTime = @lt WHERE EmailAddress = '" + HttpUtility.HtmlEncode(emailaddress.Text.ToString().Trim()) + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@fa", 0);
                            cmd.Parameters.AddWithValue("@lt", DateTime.Now);
                            cmd.Connection = connection;
                            connection.Open();
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        protected int getnofail()
        {
            var nom = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select FailedAttempts FROM Account WHERE EmailAddress='" + HttpUtility.HtmlEncode(emailaddress.Text.ToString().Trim()) + "'";
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FailedAttempts"] != DBNull.Value)
                        {
                            nom = Convert.ToInt32(reader["FailedAttempts"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return nom;
        }

        protected void counterfail()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Update Account set FailedAttempts = @fa, LockoutTime = @la WHERE EmailAddress= '" + HttpUtility.HtmlEncode(emailaddress.Text.ToString().Trim()) + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@fa", getnofail() + 1);
                            cmd.Parameters.AddWithValue("@la", DateTime.Now);
                            cmd.Connection = connection;
                            connection.Open();

                            cmd.ExecuteNonQuery();
                            connection.Close();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

    }
}