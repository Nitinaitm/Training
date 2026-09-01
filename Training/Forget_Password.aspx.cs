using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
                // Show only emp_show if the password reset was completed
                emp_show.Visible = true;
                enter_otp.Visible = false;
                home.Visible = false;
                Session["ResetCompleted"] = true;
                // Clear the session to prevent unwanted behavior on future reloads
            }
        }
        protected void btnSendOTP_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate();

                string empID = txtEmpID.Text.Trim();
                string mobile = "";
                enter_otp.Visible = false;
                emp_show.Visible = true;
                home.Visible = false;
                Encryptor2 enc = new Encryptor2();

                string captcha = Session["CaptchaCode"]?.ToString();
                if (Session["CaptchaCode"] != null && txtCaptcha.Text == Session["CaptchaCode"].ToString())
                {
                    // lblMsg.Text = "CAPTCHA Verified Successfully! ✅";
                    lblMessage2.ForeColor = System.Drawing.Color.Green;

                    // Proceed with login authentication...
                }
                else
                {
                    lblMessage2.Text = "CAPTCHA Verification Failed! ❌ Try again.";
                    lblMessage2.ForeColor = System.Drawing.Color.Red;

                    return;
                }


                string query = "SELECT EmpID, EmpName, EmpDesignation, MobileNo, EmailId, LoginIDUserID FROM Login inner Join EmpBasicMaster on Login.CorrespondingEmpID = EmpBasicMaster.EmpID WHERE LoginIDUserID = @EmpID";
                SqlParameter[] parameters1 = new SqlParameter[]
               {
                      new SqlParameter("@EmpID", empID)


               };
                DataTable dt = cls.GetDataTable(query, parameters1);
                if (dt.Rows.Count <= 0)
                {
                    lblMessage2.Text = "Invalid User ID/ EmpID";
                    return;
                }
                else if (dt.Rows.Count > 0)
                {


                    mobile = dt.Rows[0]["MobileNo"] != DBNull.Value ? dt.Rows[0]["MobileNo"].ToString() : string.Empty;
                   
                    if (mobile == null || mobile == "")
                    {
                        lblMessage2.Text = "Mobile No. not Registered. Kindly contact Administrator";
                        return;
                    }
                }
                btnSendOTP.Text = "Sending...";
                btnSendOTP.Enabled = false;



                string empName = dt.Rows[0]["EmpName"].ToString();
                Session["Name"] = empName;
                string designation = dt.Rows[0]["EmpDesignation"].ToString();
                Session["designation"] = designation;
                string otp_mobile = SendSMSToMobile(mobile);


                string pre = mobile.Substring(0, 2);
                string post = mobile.Substring(8, 2);
                string mobShow = pre + "******" + post;

                Random rand = new Random();
               // string otpEmail = rand.Next(100000, 999999).ToString(); // 6-digit OTP
                Session[otpSessionKeyMobile] = otp_mobile; // Store OTP in session for validation


                lblMessage1.Text = "OTP has been sent to Your Mobile No.:" + mobShow;
                enter_otp.Visible = true;
                emp_show.Visible = false;
                home.Visible = false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error sending OTP: " + ex.Message;


            }

        }



        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblMessage1.Text = "";
            try
            {
                if (Convert.ToBoolean(Session["ResetCompleted"]) == false)
                {
                    Response.Redirect("ForgotPassword.aspx");
                }
                string empID = txtEmpID.Text.Trim();
                string otpMobile = txtOTPMobile.Text.Trim();
                string newPassword = txtNewPassword.Text.Trim();
                string confirmPassword = txtConfirmPassword.Text.Trim();


                if (Session["OTPMobile"] == null || otpMobile != Session["OTPMobile"].ToString())
                {
                    lblMessage1.Text = "Invalid Mobile OTP. Please try again.";
                    return;
                }
                // Validate password strength
                if (!IsValidPassword(newPassword))
                {
                    lblMessage3.Text = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character (!@#$%^&*_).";
                    return;
                }

                // Ensure passwords match
                if (newPassword != confirmPassword)
                {
                    lblMessage3.Text = "Passwords do not match.";
                    return;
                }
                string name = Session["Name"].ToString();
                string designation = Session["designation"].ToString();
                Encryptor2 enc = new Encryptor2();
                string password_enc = enc.Encrypt(newPassword);
                string re_enc = enc.Encrypt("N");
                // Update password in database

                string query = "UPDATE Login SET Password = @Password, re = @re_enc WHERE LoginIDUserID = @EmpID";
                List<SqlParameter> updateParameters = new List<SqlParameter>();
                updateParameters.Add(new SqlParameter("@EmpID", empID));
                updateParameters.Add(new SqlParameter("@Password", password_enc));
                updateParameters.Add(new SqlParameter("@re_enc", re_enc));



                int rowsUpdated = cls.ExecuteSql(query, updateParameters.ToArray());
                if (rowsUpdated > 0)
                {
                    lblMessage.Text = "Password reset successfully!";
                  //  string toMail = Session["email"]?.ToString();
                    
                    
                  //  Email_Transaction emailTrans = new Email_Transaction();
                   // emailTrans.SendMail_Password(toMail, name, designation, empID);

                    Session[otpSessionKeyMobile] = null; // Clear OTP session
                
                    Session.Remove("OTPMobile");
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
                    Session["ResetCompleted"] = false;
                }
                else
                {
                    lblMessage3.Text = "Error resetting password.";
                }
            }
            catch (Exception ex)
            {
                // Show a generic error message and optionally log the exception
                lblMessage3.Text = "An unexpected error occurred. Please try again later.";
                // You can also log it like: LogError(ex);
            }
        }
        private bool IsValidPassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => "!@#$%^&*_".Contains(c));
        }
        private string SendSMSToMobile(string toPhoneNumber)
        {
            clsDataAccess cls = new clsDataAccess();
            string message = "";

            string otp = string.Empty;
            string to = "";

            Random r = new Random();
            int num = 1111;
          //  int num = r.Next(1000, 9999);
            otp = num.ToString();

            message = otp;
            to = toPhoneNumber.ToString().Trim();

            try
            {
                if (!string.IsNullOrEmpty(to))
                {
                    //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
                    //string enc = Encoding.UTF8.GetString(bytes);

                    ////HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://push3.aclgateway.com/servlet/com.aclwireless.pushconnectivity.listeners.TextListener?appid=bsphalt&userId=bsphalt&pass=bsphal_09&contenttype=1&from=BSPHCI&to=" + to + "&text=One Time Password (OTP) for APAR login is :" + message + "- BSPHCL&alert=1&selfid=true");
                    //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://api.pinnacle.in/index.php/sms/urlsms?sender=BSPHCE&numbers=" + to + "&messagetype=TXT&message=One Time Password (OTP) for APAR login is :" + message + "- BSPHCL&response=Y&username=dbabsphcl&pass=Dba$7803");

                    //HttpWebResponse myResp = (HttpWebResponse)req.GetResponse();
                    //StreamReader respStreamReader = new StreamReader(myResp.GetResponseStream());
                    //string responseString = respStreamReader.ReadToEnd();
                    //respStreamReader.Close();
                    //myResp.Close();



                }
            }

            catch
            {

            }

            return message;




        }
    }
}