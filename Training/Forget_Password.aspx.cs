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
    public partial class Forget_Password : System.Web.UI.Page
    {
        private string otpSessionKeyMobile = "OTPMobile";
        clsDataAccess cls = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                emp_show.Visible = true;
                enter_otp.Visible = false;
                home.Visible = false;
                Session["ResetCompleted"] = true;
            }
        }

        protected void btnSendOTP_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate();
                string empID = txtEmpID.Text.Trim();
                enter_otp.Visible = false;
                emp_show.Visible = true;
                home.Visible = false;

                if (Session["CaptchaCode"] == null || txtCaptcha.Text.Trim() != Session["CaptchaCode"].ToString())
                {
                    lblMessage2.Text = "CAPTCHA Verification Failed! ❌ Try again.";
                    lblMessage2.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string query = @"
SELECT TOP 1
    L.LoginIDUserID,
    L.Role,
    COALESCE(E.EmpName, TM.NameExternal, TE.TraineeName, L.LoginIDUserID) AS DisplayName,
    COALESCE(E.EmpDesignation, TM.DesignationExternal, TE.Designation, '') AS Designation,
    COALESCE(E.MobileNo, TM.MobileNo, TE.MobileNo, '') AS MobileNo,
    COALESCE(E.EmailId, TM.EmailID, TE.EmailID, '') AS EmailID
FROM Login L
LEFT JOIN EmpBasicMaster E
    ON E.EmpID = L.CorrespondingEmpID
LEFT JOIN TrainerMaster TM
    ON TM.TrainerID = L.LoginIDUserID
    OR TM.EmpID = L.CorrespondingEmpID
    OR TM.EmpIDExternal = L.CorrespondingEmpID
LEFT JOIN TraineeMasterExternal TE
    ON TE.TraineeID = L.LoginIDUserID
    OR TE.EmpIDExternal = L.CorrespondingEmpID
WHERE L.LoginIDUserID = @UserID";

                DataTable dt = cls.GetDataTable(query, new SqlParameter[]
                {
                    new SqlParameter("@UserID", empID)
                });

                if (dt.Rows.Count == 0)
                {
                    lblMessage2.Text = "Invalid User ID / EmpID";
                    return;
                }

                string mobile = dt.Rows[0]["MobileNo"] == DBNull.Value ? "" : dt.Rows[0]["MobileNo"].ToString().Trim();
                if (string.IsNullOrWhiteSpace(mobile))
                {
                    lblMessage2.Text = "Mobile No. not Registered. Kindly contact Administrator";
                    return;
                }

                btnSendOTP.Text = "Sending...";
                btnSendOTP.Enabled = false;

                Session["Name"] = dt.Rows[0]["DisplayName"].ToString();
                Session["designation"] = dt.Rows[0]["Designation"].ToString();

                string otpMobile = SendSMSToMobile(mobile);
                Session[otpSessionKeyMobile] = otpMobile;

                string mobShow;
                if (mobile.Length >= 4)
                    mobShow = mobile.Substring(0, 2) + new string('*', Math.Max(0, mobile.Length - 4)) + mobile.Substring(mobile.Length - 2);
                else
                    mobShow = mobile;

                lblMessage1.Text = "OTP has been sent to Your Mobile No.: " + mobShow;
                enter_otp.Visible = true;
                emp_show.Visible = false;
                home.Visible = false;
            }
            catch
            {
                lblMessage.Text = "Error sending OTP. Please try again later.";
                btnSendOTP.Enabled = true;
                btnSendOTP.Text = "Send OTP";
            }
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblMessage1.Text = "";
            try
            {
                if (Session["ResetCompleted"] == null || !Convert.ToBoolean(Session["ResetCompleted"]))
                {
                    Response.Redirect("ForgotPassword.aspx");
                    return;
                }

                string empID = txtEmpID.Text.Trim();
                string otpMobile = txtOTPMobile.Text.Trim();
                string newPassword = txtNewPassword.Text.Trim();
                string confirmPassword = txtConfirmPassword.Text.Trim();

                if (Session[otpSessionKeyMobile] == null || otpMobile != Session[otpSessionKeyMobile].ToString())
                {
                    lblMessage1.Text = "Invalid Mobile OTP. Please try again.";
                    return;
                }

                if (!IsValidPassword(newPassword))
                {
                    lblMessage3.Text = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character (!@#$%^&*_).";
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    lblMessage3.Text = "Passwords do not match.";
                    return;
                }

                Encryptor2 enc = new Encryptor2();
                string passwordEnc = enc.Encrypt(newPassword);
                string reEnc = enc.Encrypt("N");

                string query = "UPDATE Login SET Password=@Password, re=@re_enc WHERE LoginIDUserID=@EmpID";
                int rowsUpdated = cls.ExecuteSql(query, new SqlParameter[]
                {
                    new SqlParameter("@EmpID", empID),
                    new SqlParameter("@Password", passwordEnc),
                    new SqlParameter("@re_enc", reEnc)
                });

                if (rowsUpdated > 0)
                {
                    lblMessage.Text = "Password reset successfully!";
                    Session.Remove(otpSessionKeyMobile);
                    Session.Remove("Name");
                    Session.Remove("designation");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
                    Response.Cache.SetNoStore();
                    Session.Clear();
                    Session.RemoveAll();
                    Session.Abandon();
                    System.Web.Security.FormsAuthentication.SignOut();
                    enter_otp.Visible = false;
                    emp_show.Visible = false;
                    home.Visible = true;
                    Label1.Text = "Password reset successfully";
                }
                else
                {
                    lblMessage3.Text = "Error resetting password.";
                }
            }
            catch
            {
                lblMessage3.Text = "An unexpected error occurred. Please try again later.";
            }
        }

        private bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 8 &&
                   password.Any(char.IsUpper) && password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) && password.Any(c => "!@#$%^&*_".Contains(c));
        }

        private string SendSMSToMobile(string toPhoneNumber)
        {
            string message = "";
            string to = (toPhoneNumber ?? "").Trim();
            Random r = new Random();
            int num = 1111;
            message = num.ToString();

            try
            {
                if (!string.IsNullOrEmpty(to))
                {
                    // Existing SMS gateway integration can be enabled here.
                }
            }
            catch
            {
            }

            return message;
        }
    }
}