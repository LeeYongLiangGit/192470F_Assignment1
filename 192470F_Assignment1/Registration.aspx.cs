using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text.RegularExpressions; // for Regular expression
using System.Drawing; // for change of color

using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace _192470F_Assignment1
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDB_192470F = System.Configuration.ConfigurationManager.ConnectionStrings["192470FDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        DateTime test = new DateTime(1800, 12, 30);

        protected void Page_Load(object sender, EventArgs e)
        {

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

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            bool validInput = validateinput();

            if (validInput)
            {
                string pwd = tb_password.Text.ToString().Trim();

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

                createAccount();

                firstName.Text = "";
                lastName.Text = "";
                email.Text = "";
                CardNum.Text = "";
                ValidDate.Text = "";
                dob.Text = "";
                IbI_pwdchecker2.Text = "";
                emailValidate.Text = "";
                passwordValidate.Text = "";
                Ans.Text = "";
                Result.Text = "Account successfully created!";
                Result.ForeColor = Color.Green;
            }
        }

        protected bool validateinput()
        {
            emailValidate.Text = "";
            IbI_pwdchecker2.Text = "";
            passwordValidate.Text = "";
            bool result = false;

            // implement codes for the button event
            // Extract data from textbox
            int scores = checkPassword(tb_password.Text);
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

            if (checkEmail(email.Text.ToString()))
            {
                emailValidate.Text = "Email already exist!";
                emailValidate.ForeColor = Color.Red;
            }
            else
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email.Text.ToString());
                if (!match.Success)
                {
                    emailValidate.Text = "Invalid email format!";
                    emailValidate.ForeColor = Color.Red;
                }
            }

            if (scores < 4)
            {
                IbI_pwdchecker2.ForeColor = Color.Red;
            }

            if (cfmpassword.Text.ToString() != tb_password.Text.ToString())
            {
                passwordValidate.Text = "Password does not match!";
                passwordValidate.ForeColor = Color.Red;
            }

            if (String.IsNullOrEmpty(emailValidate.Text) && String.IsNullOrEmpty(passwordValidate.Text))
            {
                if (scores > 4)
                {
                    result = true;
                }
            }
            return result;
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDB_192470F))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FirstName, @LastName, @CardNumber, @ExpirationDate, @Email, @PasswordHash, @PasswordSalt, @PasswordAge, @PasswordHistory, @Dob, @IV, @Key, @SecurityAns, @LoginAttempt, @LockoutStartDate)"))
                {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@FirstName", firstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", lastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", email.Text.Trim());
                            cmd.Parameters.AddWithValue("@CardNumber", Convert.ToBase64String(encryptData(CardNum.Text.Trim())));
                            cmd.Parameters.AddWithValue("@ExpirationDate", Convert.ToBase64String(encryptData(ValidDate.Text.Trim())));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@PasswordAge", DateTime.Now);
                            cmd.Parameters.AddWithValue("@PasswordHistory", Convert.ToBase64String(encryptData(tb_password.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Dob", dob.Text.Trim());
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@SecurityAns", Ans.Text.ToString().Trim());
                            cmd.Parameters.AddWithValue("@LoginAttempt", 0);
                            cmd.Parameters.AddWithValue("@LockoutStartDate", test);


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
    }
}
