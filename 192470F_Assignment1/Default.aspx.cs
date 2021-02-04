using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _192470F_Assignment1
{
    public partial class _Default : Page
    {
        string MYDB_192470F = System.Configuration.ConfigurationManager.ConnectionStrings["192470FDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null &&
                Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx");
                }

                else if (maxPwdAge(Session["UserID"].ToString()))
                {
                    Response.Redirect("UpdatePassword.aspx?value=You are required to change your password now, according to the security policy!");
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

            if (Request.QueryString["value"] != null)
            {
                Result2.Text = HttpUtility.HtmlEncode(Request.QueryString["value"]);
                Result2.ForeColor = Color.Blue;
            }
        }

        protected DateTime getPasswordAge(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDB_192470F);
            DateTime s = new DateTime();

            try
            {
                string sql = "Select PasswordAge FROM Account WHERE Email=@USERID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@USERID", userid);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDAGE"] != null)
                        {
                            if (reader["PASSWORDAGE"] != DBNull.Value)
                            {
                                s = Convert.ToDateTime(reader["PASSWORDAGE"]);
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

        public bool minPwdAge(string userid)
        {
            bool result = false;
            DateTime age = getPasswordAge(userid);

            if (DateTime.Now.Date > age.Date)
            {
                result = true;
            }
            else if (DateTime.Now.TimeOfDay.TotalMinutes >= (age.TimeOfDay.TotalMinutes + 5))
            {
                result = true;
            }
            return result;
        }

        public bool maxPwdAge(string userid)
        {
            bool result = false;
            DateTime age = getPasswordAge(userid);

            if (DateTime.Now.Date > age.Date)
            {
                result = true;
            }
            else if (DateTime.Now.TimeOfDay.TotalMinutes >= (age.TimeOfDay.TotalMinutes + 15))
            {
                result = true;
            }
            return result;
        }
    }
}
