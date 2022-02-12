using System;
using System.Collections.Generic;
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
    public partial class registration : System.Web.UI.Page
    {
        // state the variables that will be needed
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        // for password
        static string finalHash;
        static string salt;

        // for encrypting/decrypting the credit card
        byte[] Key;
        byte[] IV;

        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // clicking the submit button
        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                StartUpLoad();
            }
            else
            {
                // error
                Response.Redirect("errorpage.aspx", false);
            }
        }

        // start validating and creating of account
        private void StartUpLoad()
        {
            int scores = checkPassword(tb_password.Text);
            string status = "";
            // validation of password
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
            passworderror.Text = "Status : " + status;

            if (scores < 4)
            {
                passworderror.ForeColor = Color.Red;
                return;
            }
            else
            {
                if (existEmail(HttpUtility.HtmlEncode(email.Text.ToString().Trim())))
                {
                    System.Diagnostics.Debug.WriteLine("Email exist liao");

                    lblMessage.Text = "Email already exist";
                    lblMessage.ForeColor = Color.Red;
                }
                else
                {
                    // validation of image uploaded
                    passworderror.ForeColor = Color.Green;
                    string imageName = PhotoUpload.FileName;
                    //sets the image path  
                    string imagePath = "ProfilePictureStorage/" + imageName;

                    int imgSize = PhotoUpload.PostedFile.ContentLength;

                    //validates the posted file before saving  
                    if (PhotoUpload.PostedFile != null && PhotoUpload.PostedFile.FileName != "")
                    {
                        if (PhotoUpload.PostedFile.ContentLength > 100000)
                        {
                            pictureerror.Text = "File size too big. Please upload a smaller size, lesser than 100000kb";
                            pictureerror.ForeColor = Color.Red;
                        }
                        else
                        {
                            //then save it to the Folder  
                            PhotoUpload.SaveAs(Server.MapPath(imagePath));
                            imgpreview.ImageUrl = "~/" + imagePath;


                            // password hashing
                            string pwd = HttpUtility.HtmlEncode(tb_password.Text.ToString().Trim());
                            //Generate random "salt"
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];
                            //Fills array of bytes with a cryptographically strong sequence of random values.
                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);
                            SHA512Managed hashing = new SHA512Managed();
                            string pwdWithSalt = pwd + salt;
                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            finalHash = Convert.ToBase64String(hashWithSalt);
                            RijndaelManaged cipher = new RijndaelManaged();
                            cipher.GenerateKey();
                            Key = cipher.Key;
                            IV = cipher.IV;
                            if (createAccount(imagePath))
                            {
                                Response.Redirect("Login.aspx");
                            }
                            else
                            {
                                Response.Redirect("errorpage.aspx", false);
                            }
                        }

                    }
                    else
                    {
                        pictureerror.Text = "Please upload an image";
                        pictureerror.ForeColor = Color.Red;
                    }
                }

            }
        }

        // creation of account
        protected bool createAccount(string imgPath)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    // insert to only selected column
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account(Fname,Lname,CreditNo,EmailAddress,DateOfBirth,PasswordHash,PasswordSalt,ProfilePath,[IV],[Key]) VALUES(@fname,@lname,@creditno,@emailaddress,@dateofbirth,@passwordhash,@passwordsalt,@profilepath,@iv,@key)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            try
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@fname", HttpUtility.HtmlEncode(firstname.Text.Trim()));
                                cmd.Parameters.AddWithValue("@lname", HttpUtility.HtmlEncode(lastname.Text.Trim()));
                                //cmd.Parameters.AddWithValue("@Nric", encryptData(tb_nric.Text.Trim()));
                                cmd.Parameters.AddWithValue("@creditno", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(creditcard.Text.Trim().ToString()))));
                                cmd.Parameters.AddWithValue("@emailaddress", HttpUtility.HtmlEncode(email.Text.Trim()));
                                cmd.Parameters.AddWithValue("@dateofbirth", HttpUtility.HtmlEncode(dateofbirth.Text.Trim()));
                                cmd.Parameters.AddWithValue("@passwordhash", finalHash);
                                cmd.Parameters.AddWithValue("@passwordsalt", salt);
                                cmd.Parameters.AddWithValue("@profilepath", imgPath);
                                cmd.Parameters.AddWithValue("@iv", Convert.ToBase64String(IV));
                                cmd.Parameters.AddWithValue("@key", Convert.ToBase64String(Key));
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                                return true;
                            }
                            catch (SqlException ex)
                            {
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


        // for password
        protected void tb_passwword_TextChanged(object sender, EventArgs e)
        {

        }
        private int checkPassword(string password)
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


        // for v3 captcha
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

        // encrypt the credit card details
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        // check for existing email
        protected bool existEmail(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select EmailAddress FROM Account";
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["EmailAddress"] != DBNull.Value)
                        {
                            System.Diagnostics.Debug.WriteLine("are you going in here");

                            if (email == reader["EmailAddress"].ToString())
                            {
                                System.Diagnostics.Debug.WriteLine("Email exist");
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
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

    }
}