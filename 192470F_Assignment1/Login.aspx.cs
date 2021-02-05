using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _192470F_Assignment1
{
    public class MyObject
    {
        public string success { get; set; }

        public List<string> ErrorMessage { get; set; }
    }

    public partial class Login : System.Web.UI.Page
    {
        string MYDB_192470F = System.Configuration.ConfigurationManager.ConnectionStrings["192470FDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null &&
                Request.Cookies["AuthToken"] != null)
            {
                if (Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Default.aspx");
                }
            }

            if (Request.QueryString["value"] != null)
            {
                Result.Text = HttpUtility.HtmlEncode(Request.QueryString["value"]);
                Result.ForeColor = System.Drawing.Color.Blue;
            }
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            string pwd = tb_pwd.Text.ToString().Trim();
            string userid = tb_email.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userid);
            string dbSalt = getDBSalt(userid);
            int tries = getAttempt(userid);
            DateTime lockDate = getLockDate(userid);
            DateTime test = new DateTime(1800, 12, 30);
            bool lockdownResult = true;

            if (lockDate != test)
            {
                if (DateTime.Now.Date > lockDate.Date)
                {
                    lockdownResult = false;
                }
                else if (lockDate.TimeOfDay.TotalMinutes + 1 < DateTime.Now.TimeOfDay.TotalMinutes)
                {
                    lockdownResult = false;
                }

                if (!lockdownResult)
                {
                    updateAttempt(userid, 0);
                    updateLockDate(userid, test);
                    tries = getAttempt(userid);
                }
            }

            if (tries < 3)
            {
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdWithSalt = pwd + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);

                        if (ValidateCaptcha())
                        {
                            if (userHash.Equals(dbHash))
                            {
                                Session["UserID"] = userid;

                                // creates a new GUID and save into the session
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                // Now create a new cookie with this guid value
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                updateAttempt(userid, 0);
                                updateLockDate(userid, test);

                                Response.Redirect("Default.aspx", false);
                            }
                            else
                            {
                                int attempts = tries + 1;
                                updateAttempt(userid, attempts);
                                lblMessage.Text = "Invalid Credentials. Please try again!" + "<br/>" + "Attempt:" + attempts;
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = "Please fill in your username and password!";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { }
            }
            else
            {
                if (lockDate == test)
                {
                    updateLockDate(userid, DateTime.Now);
                }

                lockDate = getLockDate(userid);
                lblMessage.Text = "Your account is currently lockout! You will need to wait until " + lockDate.AddMinutes(1) + " before it is unlocked.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }
        public bool ValidateCaptcha()
        {
            bool result = true;

            // When user submits the recaptcha form, the user gets a response POST parameter.
            // captchaResponse consist of the user click pattern. Behaviour analytics! AI :)
            string captchaResponse = Request.Form["g-recaptcha-response"];

            // To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=Your captcha key &response=" + captchaResponse);

            try
            {
                // Codes to receive the Response in JSON format from the Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        // The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //// To show the JSON response string for learning purpose
                        //lbl_gScore.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        // Create jsonObject to handle the response e.g success or Error
                        // Deserialise Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        // Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected string getDBHash(string userid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDB_192470F);
            
            try
            {
                string sql = "Select PasswordHash FROM Account WHERE Email=@USERID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@USERID", userid);
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

        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDB_192470F);
            
            try
            {
                string sql = "Select PasswordSalt FROM Account WHERE Email=@USERID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@USERID", userid);
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

        protected int getAttempt(string userid)
        {
            int s = 0;
            SqlConnection connection = new SqlConnection(MYDB_192470F);
            
            try
            {
                string sql = "Select LoginAttempt FROM Account WHERE Email=@USERID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@USERID", userid);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LoginAttempt"] != null)
                        {
                            if (reader["LoginAttempt"] != DBNull.Value)
                            {
                                s = Convert.ToInt32(reader["LoginAttempt"]);
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

        protected DateTime getLockDate(string userid)
        {
            DateTime s = new DateTime();
            SqlConnection connection = new SqlConnection(MYDB_192470F);

            try
            {
                string sql = "Select LockoutStartDate FROM Account WHERE Email=@USERID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@USERID", userid);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LockoutStartDate"] != null)
                        {
                            if (reader["LockoutStartDate"] != DBNull.Value)
                            {
                                s = Convert.ToDateTime(reader["LockoutStartDate"]);
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

        protected void updateAttempt(string userid, int tries)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDB_192470F))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LoginAttempt = @Tries WHERE Email = @Email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", userid);
                            cmd.Parameters.AddWithValue("@Tries", tries);


                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void updateLockDate(string userid, DateTime Date)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDB_192470F))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LockoutStartDate = @Date WHERE Email = @Email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", userid);
                            cmd.Parameters.AddWithValue("@Date", Date);

                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
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
