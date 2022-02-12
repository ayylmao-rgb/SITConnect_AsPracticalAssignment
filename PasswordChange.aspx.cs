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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_AsPracticalAssignment
{
    public partial class PasswordChange : System.Web.UI.Page
    {
        string MYDBConnection = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        string stringitem = null;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // validate if they are logged in, else they will be redirected
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

        // when change button is clicked
        protected void changebtn(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                // validate password if correct format
                int scores = validatePassword(newpassword.Text);
                string status = "";
                switch (scores)
                {
                    case 1:
                        status = "Very Weak";
                        break;
                    case 2:
                        status = "Weak";
                        break;
                    case 3:
                        status = "Medium";
                        break;
                    case 4:
                        status = "Strong";
                        break;
                    case 5:
                        status = "Excellent";
                        break;
                    default:
                        break;
                }
                pwdchecker.Text = "Status : " + status;
                if (scores < 4)
                {
                    pwdchecker.ForeColor = Color.Red;
                    return;
                }
                else
                {
                    SqlConnection con = new SqlConnection(MYDBConnection);
                    con.Open();
                    string emailaddress = (string)Session["userID"];
                    string pwd = HttpUtility.HtmlEncode(currentpassword.Text);
                    SHA512Managed hashing = new SHA512Managed();
                    string dbHash = getDBHash(emailaddress);
                    string dbSalt = getDBSalt(emailaddress);
                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (userHash.Equals(dbHash))
                            {
                                string newpwd = HttpUtility.HtmlEncode(newpassword.Text.ToString().Trim());
                                //Generate random "salt"
                                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                byte[] saltByte = new byte[8];
                                //Fills array of bytes with a cryptographically strong sequence of random values.
                                rng.GetBytes(saltByte);
                                salt = Convert.ToBase64String(saltByte);
                                SHA512Managed hashingg = new SHA512Managed();
                                string pwdWithSalt2 = newpwd + salt;
                                byte[] plainHash = hashingg.ComputeHash(Encoding.UTF8.GetBytes(newpwd));
                                byte[] hashWithSalt2 = hashingg.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt2));
                                finalHash = Convert.ToBase64String(hashWithSalt2);
                                RijndaelManaged cipher = new RijndaelManaged();
                                cipher.GenerateKey();
                                Key = cipher.Key;
                                IV = cipher.IV;
                                if (passwordchange())
                                {
                                    lbl_msg.Text = "Password changed Successfully";

                                }
                                else
                                {
                                    Response.Redirect("errorpage.aspx", false);

                                }
                            }
                            else
                            {
                                lbl_msg.Text = "incorrect current password";
                                lbl_msg.ForeColor = Color.Red;
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

        // change password
        protected bool passwordchange()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnection))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET [PasswordHash]=@hash,[PasswordSalt]=@Salt WHERE EmailAddress='" + Session["UserID"].ToString() + "'"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            try
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@hash", finalHash);
                                cmd.Parameters.AddWithValue("@Salt", salt);
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                                return true;
                            }
                            catch (SqlException ex)
                            {
                                lblMessage.Text = ex.ToString();
                                return false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
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

        // validate password
        private int validatePassword(string password)
        {
            int score = 0;

            // include your implementation here

            //score 0 very weak;
            // if length of password is less than 8 chars
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            // score 2 weak
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            // score 3 medium
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            // score 4 strong
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            // score 5 excellent
            if (Regex.IsMatch(password, "(?=.*[^A-Za-z0-9])"))
            {
                score++;
            }

            return score;
        }

        //get password hash
        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnection);
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

        //get password salt
        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnection);
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
    }
}