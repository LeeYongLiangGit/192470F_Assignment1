using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _192470F_Assignment1
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        string MYDB_192470F = System.Configuration.ConfigurationManager.ConnectionStrings["192470FDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        byte[] history = null;
        string[] historyresult;
        string newHistory;

        protected void Page_Load(object sender, EventArgs e)
        {
            id1.Visible = false;
        }

        protected void getPasswordHistory(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDB_192470F);

            try
            {
                string sql = "Select * FROM Account WHERE Email=@USERID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@USERID", userid);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }

                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }

                        if (reader["PasswordHistory"] != DBNull.Value)
                        {
                            history = Convert.FromBase64String(reader["PasswordHistory"].ToString());
                            historyresult = decryptData(history).Split(',');
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
        }

        protected void btn_reset_Click(object sender, EventArgs e)
        {
            if (checkEmail(Email.Text.ToString().Trim()))
            {
                getPasswordHistory(Email.Text.ToString().Trim());

                EmailCheck.Text = "";
                id1.Visible = true;

                bool validInput = validateinput();

                if (validInput)
                {
                    string pwd = new_pwd.Text.ToString().Trim();

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

                    if (historyresult.Length > 1)
                    {
                        if (historyresult[1] != new_pwd.Text.ToString())
                        {
                            newHistory = historyresult[1] + "," + new_pwd.Text.ToString();
                        }
                        else
                        {
                            newHistory = decryptData(history);
                        }
                    }
                    else
                    {
                        if (historyresult[0] != new_pwd.Text.ToString())
                        {
                            newHistory = historyresult[0] + "," + new_pwd.Text.ToString();
                        }
                        else
                        {
                            newHistory = decryptData(history);
                        }
                    }

                    updatePassword(Email.Text.ToString().Trim());

                    Response.Redirect("Login.aspx?value=Password reset!, please login again!");
                }
            }
            else
            {
                id1.Visible = false;
                EmailCheck.Text = "Please make sure that you have entered the correct email!";
                EmailCheck.ForeColor = Color.Red;
            }
        }

        protected void updatePassword(string userid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDB_192470F))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt, PasswordAge = @PasswordAge, PasswordHistory = @PasswordHistory WHERE Email = @Email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", userid);
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@PasswordAge", DateTime.Now);
                            cmd.Parameters.AddWithValue("@PasswordHistory", Convert.ToBase64String(encryptData(newHistory)));

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

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                // create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                // create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }

        protected string checkSecurityAnswer(string email)
        {
            string result = "";
            SqlConnection connection = new SqlConnection(MYDB_192470F);

            try
            {
                string sql = "select SecurityAns FROM Account WHERE Email=@EMAIL";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EMAIL", email);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["SecurityAns"] != null)
                        {
                            if (reader["SecurityAns"] != DBNull.Value)
                            {
                                result = reader["SecurityAns"].ToString();
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
            return result;
        }

        protected bool checkEmail(string email)
        {
            bool result = false;
            SqlConnection connection = new SqlConnection(MYDB_192470F);

            try
            {
                string sql = "select Email FROM Account WHERE Email=@EMAIL";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EMAIL", email);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["Email"] != null)
                        {
                            if (reader["Email"] != DBNull.Value)
                            {
                                result = true;
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
            return result;
        }

        private int checkPassword(string password)
        {
            int score = 0;

            // Score 0 is weak!
            // If length of password is less than 8 characters
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            // Score 2 weak
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            // Score 3 medium
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            // Score 4 strong
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            // Score 5 excellent
            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }

            return score;
        }

        protected bool validateinput()
        {
            IbI_pwdchecker2.Text = "";
            passwordValidate.Text = "";
            SecurityAnsCheck.Text = "";
            bool result = false;

            // implement codes for the button event
            // Extract data from textbox
            int scores = checkPassword(new_pwd.Text);
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

            IbI_pwdchecker2.Text = "Status : " + status;
            IbI_pwdchecker2.ForeColor = Color.Green;

            if (Ans.Text.ToString().Trim() != checkSecurityAnswer(Email.Text.ToString().Trim()))
            {
                SecurityAnsCheck.Text = "Your answer is wrong! Please try again!";
                SecurityAnsCheck.ForeColor = Color.Red;
            }

            if (scores < 4)
            {
                IbI_pwdchecker2.ForeColor = Color.Red;
            }

            if (cfm_pwd.Text.ToString() != new_pwd.Text.ToString())
            {
                passwordValidate.Text = "Password does not match!";
                passwordValidate.ForeColor = Color.Red;
            }

            if (String.IsNullOrEmpty(passwordValidate.Text) && Ans.Text.ToString().Trim() == checkSecurityAnswer(Email.Text.ToString().Trim()))
            {
                if (scores > 4)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
