using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        clsDataAccess cls = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("Default.aspx"); // Redirect if session is not available
            }
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                string userID = Session["UserId"].ToString();
                string newPassword = txtNewPassword.Text.Trim();
                string confirmPassword = txtConfirmPassword.Text.Trim();

                if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    lblMessage.Text = "Please enter and confirm your new password.";
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    lblMessage.Text = "Passwords do not match!";
                    return;
                }

                Encryptor2 encryptor = new Encryptor2();

                
                string encryptedPassword = encryptor.Encrypt(newPassword);

                string isFirstLogin = encryptor.Encrypt("N");

                string sql = @"UPDATE [Login] SET Password = @password, re = @isFirstLogin WHERE LoginIDUserID = @loginIDUserID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@password", encryptedPassword),
                    new SqlParameter("@isFirstLogin", isFirstLogin),
                    new SqlParameter("@loginIDUserID", userID)
                };

                // Execute query securely
                int status = cls.ExecuteSql(sql, parameters);
                string toMail = "";
                string name = "";
                string designation = "";
                string empID = "";

                if (status > 1)
                {
                    string sql1 = @"SELECT EmpID, EmpName, EmpDesignation, MobileNo, EmailId, LoginIDUserID, CorrespondingEmpID FROM Login inner Join EmpBasicMaster on Login.CorrespondingEmpID = EmpBasicMaster.EmpID WHERE LoginIDUserID = @loginIDUserID";

                    SqlParameter[] parameters1 = new SqlParameter[]
                    {
                    new SqlParameter("@loginIDUserID", userID)
                    };

                    // Execute query securely
                    DataTable dt = cls.GetDataTable(sql1, parameters1);
                    if (dt.Rows.Count > 0)
                    {


                        string email = dt.Rows[0]["EmailId"] != DBNull.Value ? dt.Rows[0]["EmailId"].ToString() : string.Empty;
                        Encryptor2 encry = new Encryptor2();
                        toMail = encry.Decrypt(email);
                        name = dt.Rows[0]["EmpName"] != DBNull.Value ? dt.Rows[0]["EmpName"].ToString() : string.Empty;
                        designation = dt.Rows[0]["EmpDesignation"] != DBNull.Value ? dt.Rows[0]["EmpDesignation"].ToString() : string.Empty;
                        empID = dt.Rows[0]["CorrespondingEmpID"] != DBNull.Value ? dt.Rows[0]["CorrespondingEmpID"].ToString() : string.Empty;
                    }

                   // Email_Transaction emailTrans = new Email_Transaction();
                   // emailTrans.SendMail_Password(toMail, name, designation, empID);
                }
                
                Session.Clear();
                Session.Abandon();

                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["AuthCookie"].Expires = DateTime.Now.AddDays(-1);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetNoStore();
                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();

                System.Web.Security.FormsAuthentication.SignOut();
                // Redirect to login page
                // Response.Redirect("Default.aspx?msg=PasswordChanged");
                ScriptManager.RegisterStartupScript(this, GetType(), "showMessageAndRedirect", "alert('Your password has been reset successfully. Redirecting to login page...'); setTimeout(function() { window.location.href='Default.aspx'; });", true);

              

            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
    }
}